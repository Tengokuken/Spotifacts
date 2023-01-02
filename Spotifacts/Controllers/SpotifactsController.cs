using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spotifacts.APICalls;
using Spotifacts.Models;
using SpotifyAPI.Web;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Spotifacts.Controllers
{
    public class SpotifactsController : Controller
    {
        private readonly ILogger<SpotifactsController> _logger;
        private readonly IConfiguration _config;

        public SpotifactsController(ILogger<SpotifactsController> logger,
                                    IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return GetAuthURI();
        }

        public async Task<IActionResult> Report()
        {
            
            var response = await new OAuthClient().RequestToken(
              new AuthorizationCodeTokenRequest(_config["clientID"], _config["clientSecret"], RouteData.Values["id"] + Request.Query["code"], new Uri("https://localhost:44374/Spotifacts/Report"))
            );
            var spotify = new SpotifyClient(response.AccessToken);
            var user = await spotify.UserProfile.Current();
            // Generate report
            ReportModel spotifyReport = await GenerateReport(spotify);
            spotifyReport.user = user.DisplayName;     
            ViewData["report"] = spotifyReport;
            // TODO: Also important for later: response.RefreshToken
            return View();
        }

        public async Task<ReportModel> GenerateReport(SpotifyClient spotify)
        {
            // Create short term
            PersonalizationTopRequest personalization = new PersonalizationTopRequest();
            personalization.TimeRangeParam = PersonalizationTopRequest.TimeRange.ShortTerm;
            // Doesn't seem to work. Always at 50
            //personalization.Limit = 10;
            TopListsModel shortTerm = await GenerateTopLists(spotify, personalization);
            // Create medium term
            personalization.TimeRangeParam = PersonalizationTopRequest.TimeRange.MediumTerm;
            TopListsModel mediumTerm = await GenerateTopLists(spotify, personalization);
            // Create long term
            personalization.TimeRangeParam = PersonalizationTopRequest.TimeRange.LongTerm;
            TopListsModel longTerm = await GenerateTopLists(spotify, personalization);
            // Package into report
            ReportModel report = new ReportModel
            {
                shortTerm = shortTerm,
                mediumTerm = mediumTerm,
                longTerm = longTerm,
                playlists = await PlaylistCalls.GetPlaylists(spotify)
            };
            return report;
        }
        public async Task<TopListsModel> GenerateTopLists(SpotifyClient spotify, PersonalizationTopRequest personalization)
        {
            ArrayList topArtists = await PersonalizationCalls.GetTopArtists(spotify, personalization);
            ArrayList topSongs = await PersonalizationCalls.GetTopSongs(spotify, personalization);

            TopListsModel report = new TopListsModel
            {
                topArtists = topArtists,
                topSongs = topSongs,
            };
            return report;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult GetAuthURI()
        {
            // Make sure "http://localhost:5000" is in your applications redirect URIs!
            var loginRequest = new LoginRequest(
              new Uri("https://localhost:44374/Spotifacts/Report"),
              _config["clientID"],
              LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] { 
                    Scopes.PlaylistReadPrivate, 
                    Scopes.PlaylistReadCollaborative, 
                    Scopes.UserReadPrivate, 
                    Scopes.UserLibraryRead,
                    Scopes.UserTopRead
                }
            };

            Uri uri = loginRequest.ToUri();
            Debug.WriteLine(uri);
            // Redirect user to uri via your favorite web-server
            return Redirect(uri.ToString());
        }
    }
}
