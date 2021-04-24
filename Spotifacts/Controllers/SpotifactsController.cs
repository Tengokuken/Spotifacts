using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Spotifacts.Models;
using Spotifacts.APICalls;
using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using System.Collections;

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
            Debug.WriteLine(RouteData.Values["id"] + Request.Query["code"]);
            var response = await new OAuthClient().RequestToken(
              new AuthorizationCodeTokenRequest(_config["clientID"], _config["clientSecret"], RouteData.Values["id"] + Request.Query["code"], new Uri("https://localhost:44374/Spotifacts/Report"))
            );
            // Set search limits
            PersonalizationTopRequest personalization = new PersonalizationTopRequest
            {
                TimeRangeParam = PersonalizationTopRequest.TimeRange.ShortTerm
            };
            // Doesn't seem to work. Always at 50
            //personalization.Limit = 10;
            Debug.WriteLine("walpu");
            var spotify = new SpotifyClient(response.AccessToken);
            var user = await spotify.UserProfile.Current();
            ArrayList topArtists = await PersonalizationCalls.GetTopArtists(spotify, personalization);
            ArrayList topSongs = await PersonalizationCalls.GetTopSongs(spotify, personalization);
            ArrayList playlists = await PlaylistCalls.GetPlaylists(spotify);
            ReportModel spotifyReport = new ReportModel {
                topArtists = topArtists,
                topSongs = topSongs,
                playlists = playlists,
                user = user.DisplayName
            };
            ViewData["report"] = spotifyReport;
            // TODO: Also important for later: response.RefreshToken
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Test()
        {
            string token = "BQBLXlV58VOz51pDE8r-mz5r1Gj7ujcfFcAoBHeR5ZlMlztTuYkypVUeFVzbLe__I65gjSMsyZCesh8hFhzO8xbKINElKNRLGK3YekVmbnyKUZqOEwT2YVCjUVNxqvFlT07iIuPzNXB0qjR-M0Ent1gyVVnvkyYCozWnqFItMq3HzMPrx-sS7gd9VrXHo9NIofS_FDpdXQ";
            // TODO: get using implicit grant auth
            var spotify = new SpotifyClient(token);

            var track = await spotify.Tracks.Get("29cshFKbqa3DmctSYEHjfT");
            Debug.WriteLine(track);
        }

        public ActionResult GetAuthURI()
        {
            // Make sure "http://localhost:5000" is in your applications redirect URIs!
            var loginRequest = new LoginRequest(
              new Uri("https://localhost:44374/Spotifacts/Report"),
              _config["clientID"],
              LoginRequest.ResponseType.Code
            )
            // TODO: Not sure what scopes are needed
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
            Debug.WriteLine("potato");
            //Console.WriteLine();
            return Redirect(uri.ToString());
        }
    }
}
