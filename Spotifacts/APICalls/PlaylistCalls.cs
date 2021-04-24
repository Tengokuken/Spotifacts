using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using SpotifyAPI.Web;

namespace Spotifacts.APICalls
{
    public class PlaylistCalls
    {
        public static async Task<ArrayList> GetPlaylists(SpotifyClient spotify)
        {
            ArrayList playlists = new ArrayList();
            try
            {
                var userPlaylists = await spotify.Playlists.CurrentUsers();
                await foreach (var playlist in spotify.Paginate(userPlaylists))
                    playlists.Add(playlist);
            }
            catch (APIException e)
            {
                Debug.WriteLine(e.Response);
                // Prints: invalid id
                Debug.WriteLine(e.Message);
                // Prints: BadRequest
                Debug.WriteLine(e.Response?.StatusCode);
            }
            return playlists;
        }
    }
}
