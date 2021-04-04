using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibNet.Utils
{
    public class UDPState
    {
        public UdpClient u;
        public IPEndPoint e;
        public byte[] buffer;

        public UdpClient U => u;

        public IPEndPoint E => e;

        public byte[] Buffer => buffer;
    }
}
