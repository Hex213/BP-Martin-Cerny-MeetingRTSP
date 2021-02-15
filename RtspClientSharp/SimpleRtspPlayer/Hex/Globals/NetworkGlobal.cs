using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRtspPlayer.Hex.Globals
{
    public static class NetworkGlobal
    {
        public static readonly string DefaultAgent = "RtspClientSharp - HexModif";
        public static readonly byte MinIdLength = 3;
        public static readonly string RtspPrefix = "rtsp://";
        public static readonly string HttpPrefix = "http://";
    }
}
