using System.Net.Sockets;
using LibRtspClientSharp.Hex;

namespace RtspClientSharp.Utils
{
    static class NetworkClientFactory
    {
        private const int TcpReceiveBufferDefaultSize = 64 * 1024;
        private const int UdpReceiveBufferDefaultSize = 128 * 1024;
        private const int SIO_UDP_CONNRESET = -1744830452;
        private static readonly byte[] EmptyOptionInValue = { 0, 0, 0, 0 };

        public static int GetTcpReceiveBufferDefaultSize => TcpReceiveBufferDefaultSize;
        public static int GetUdpReceiveBufferDefaultSize => UdpReceiveBufferDefaultSize;
        public static int GetSIO => SIO_UDP_CONNRESET;
        public static byte[] GetEmptyOptInVal => EmptyOptionInValue;

        public static Socket CreateTcpClient()
        {
            Socket socket;
            
            socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = NetworkClientFactory.GetTcpReceiveBufferDefaultSize,
                DualMode = true,
                NoDelay = true
            };

            return socket;
        }

        public static Socket CreateUdpClient()
        {
            Socket socket;

            socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveBufferSize = NetworkClientFactory.GetUdpReceiveBufferDefaultSize,
                DualMode = true
            };
            socket.IOControl((IOControlCode)NetworkClientFactory.GetSIO, NetworkClientFactory.GetEmptyOptInVal, null);

            return socket;
        }
    }
}