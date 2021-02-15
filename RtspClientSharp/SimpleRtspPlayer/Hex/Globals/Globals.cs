using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleRtspPlayer.Hex.Globals
{
    public static class Globals
    {
        public static readonly Regex rgxAZ = new Regex("[^A-Za-z]");
        public static readonly Regex rgx09b = new Regex("[^0-9.]");
        public static readonly Regex rgx09 = new Regex("[^0-9]");
    }
}
