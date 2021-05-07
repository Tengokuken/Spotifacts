using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotifacts.Models
{
    public class ReportModel
    {
        public string user { get; set; }
        public string AccessToken { get; set; }
        public ArrayList playlists { get; set; }

        public TopListsModel shortTerm { get; set; }
        public TopListsModel mediumTerm { get; set; }
        public TopListsModel longTerm { get; set; }
    }
}
