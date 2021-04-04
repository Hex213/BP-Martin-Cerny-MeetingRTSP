using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LibNet.Utils
{
    public class ClientTCP
    {
        private TcpClient handler;
        private TCPState _tcpState;
        
        private NetworkStream stream;

        public ClientTCP(TcpClient handler, TCPState tcpState)
        {
            this.handler = handler;
            this.stream = handler.GetStream();
            this._tcpState = tcpState;
        }

        public TcpClient Handler => handler;

        public TCPState TcpState => _tcpState;

        public NetworkStream Stream => stream;

        protected bool Equals(ClientTCP other)
        {
            return Equals(handler, other.handler);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientTCP) obj);
        }

        public override int GetHashCode()
        {
            return (handler != null ? handler.GetHashCode() : 0);
        }

        public static bool operator ==(ClientTCP left, ClientTCP right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClientTCP left, ClientTCP right)
        {
            return !Equals(left, right);
        }

        private sealed class HandlerEqualityComparer : IEqualityComparer<ClientTCP>
        {
            public bool Equals(ClientTCP x, ClientTCP y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.handler, y.handler);
            }

            public int GetHashCode(ClientTCP obj)
            {
                return (obj.handler != null ? obj.handler.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<ClientTCP> HandlerComparer { get; } = new HandlerEqualityComparer();
    }
}
