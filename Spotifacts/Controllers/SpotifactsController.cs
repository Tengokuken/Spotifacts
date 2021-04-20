using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotifacts.Models;
using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Spotifacts.Controllers
{
    public class SpotifactsController : Controller
    {
        private readonly ILogger<SpotifactsController> _logger;

        public SpotifactsController(ILogger<SpotifactsController> logger)
        {
            _logger = logger;
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
              new AuthorizationCodeTokenRequest("clientID", "clientSecret", RouteData.Values["id"] + Request.Query["code"], new Uri("https://localhost:44374/Spotifacts/Report"))
            );
            Debug.WriteLine("walpu");
            var spotify = new SpotifyClient(response.AccessToken);
            //SearchRequest searchRequest = new SearchRequest(SearchRequest.Types.Track, "Ignite");
            TrackRequest trackRequest = new TrackRequest();
            FullTrack fullTrack = new FullTrack();
            var a = await spotify.Tracks.Get("5ivhfpfnoHhD8rUI2t3aJG");
            var user = await spotify.UserProfile.Current();
            Debug.WriteLine(a.Name);
            Debug.WriteLine(a.Artists.First());
            Debug.WriteLine(a.Album.Name);
            Debug.WriteLine("Hello " + user.DisplayName);

            await foreach (
                var playlist in spotify.Paginate(await spotify.Playlists.CurrentUsers())
                )
            {
                Debug.WriteLine(playlist.Name);
            }
            await foreach (
                var topArtist in spotify.Paginate(await spotify.Personalization.GetTopArtists())
                )
            {
                Debug.WriteLine(topArtist.Name);
            }
            try
            {
                await foreach (
                var topSong in spotify.Paginate(await spotify.Personalization.GetTopTracks())
                )
                {
                    Debug.WriteLine(topSong.Name);
                }
            }
            catch (APIException e)
            {
                Debug.WriteLine(e.Response);
                // Prints: invalid id
                Debug.WriteLine(e.Message);
                // Prints: BadRequest
                Debug.WriteLine(e.Response?.StatusCode);
            }

            // Also important for later: response.RefreshToken
            return View(new ReportModel { spotify = spotify});
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
              "clientID",
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
