using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HexServer.Net
{
    public class DataClient
    {
        private IPEndPoint ip;
        private int portRtp = -1, portRtcp = -1;
        private bool blocked;

        public int PortRtp => portRtp;
        public int PortRtcp => portRtcp;

        public bool IsSetDataPorts()
        {
            return portRtcp > 0 && portRtp > 0;
        }

        public DataClient(IPEndPoint ip, bool blocked)
        {
            IP = ip;
            Blocked = blocked;
        }

        public bool Blocked
        {
            get => blocked;
            set => blocked = value;
        }

        public IPEndPoint IP
        {
            get => ip;
            private init => ip = value;
        }

        public void SetupClientUdp(int rtp, int rtcp)
        {
            if (rtp <= 0) throw new ArgumentOutOfRangeException(nameof(rtp));
            if (rtcp <= 0) throw new ArgumentOutOfRangeException(nameof(rtcp));

            portRtp = rtp;
            portRtcp = rtcp;
        }

        private sealed class IpEqualityComparer : IEqualityComparer<DataClient>
        {
            public bool Equals(DataClient x, DataClient y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.ip, y.ip);
            }

            public int GetHashCode(DataClient obj)
            {
                return (obj.ip != null ? obj.ip.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<DataClient> IpComparer { get; } = new IpEqualityComparer();

        protected bool Equals(DataClient other)
        {
            return Equals(ip, other.ip);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataClient) obj);
        }

        public override int GetHashCode()
        {
            return (ip != null ? ip.GetHashCode() : 0);
        }

        public static bool operator ==(DataClient left, DataClient right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataClient left, DataClient right)
        {
            return !Equals(left, right);
        }
    }
}
