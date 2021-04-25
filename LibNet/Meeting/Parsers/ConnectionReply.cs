using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LibNet.Meeting.Parsers
{
    public class ConnectionReply
    {
        public int Status { get; set; }

        public byte[] Id { get; set; }

        public IPEndPoint Ipep { get; set; }

        public byte[] AData { get; set; }
    }
}
