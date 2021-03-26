using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotifacts.Models
{
    public class ReportModel
    {
        public string AccessToken { get; set; }
        public SpotifyAPI.Web.SpotifyClient spotify;
    }
}
