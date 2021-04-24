using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using System.Collections;

namespace Spotifacts.Models
{
    public class ReportModel
    {
        public string user { get; set; }
        public string AccessToken { get; set; }
        public ArrayList topArtists { get; set; }
        public ArrayList topSongs { get; set; }
    }
}
