using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManoSoftware.SpotifyLocalApi.Model
{
    public class Track
    {
        public Resource Track_Resource { set; get; }
        public Resource Artist_Resource { set; get; }
        public Resource Album_Resource { set; get; }
        public int Length;
        public string Track_Type { set; get; }
    }
}
