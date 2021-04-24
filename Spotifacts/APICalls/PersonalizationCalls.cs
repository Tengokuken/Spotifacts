using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using SpotifyAPI.Web;

namespace Spotifacts.APICalls
{
    public class PersonalizationCalls
    {
        public static async Task<ArrayList> GetTopArtists(SpotifyClient spotify, PersonalizationTopRequest personalization)
        {
            ArrayList topArtists = new ArrayList();
            try
            {
                var topArtistList = await spotify.Personalization.GetTopArtists(personalization);
                await foreach (var topArtist in spotify.Paginate(topArtistList))
                    topArtists.Add(topArtist);
            }
            catch (APIException e)
            {
                Debug.WriteLine(e.Response);
                // Prints: invalid id
                Debug.WriteLine(e.Message);
                // Prints: BadRequest
                Debug.WriteLine(e.Response?.StatusCode);
            }
            return topArtists;
        }
        
        public static async Task<ArrayList> GetTopSongs(SpotifyClient spotify, PersonalizationTopRequest personalization)
        {
            ArrayList topSongs = new ArrayList();
            try
            {
                var topSongList = await spotify.Personalization.GetTopTracks(personalization);
                await foreach (var topSong in spotify.Paginate(topSongList))
                    topSongs.Add(topSong);
            }
            catch (APIException e)
            {
                Debug.WriteLine(e.Response);
                // Prints: invalid id
                Debug.WriteLine(e.Message);
                // Prints: BadRequest
                Debug.WriteLine(e.Response?.StatusCode);
            }
            return topSongs;
        }
    }
}
