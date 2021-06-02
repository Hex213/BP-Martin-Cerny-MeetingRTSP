using System;
using System.Collections.Generic;
using System.Text;
using LibHexCryptoStandard.Hashs;
using LibHexUtils.Arrays;
using LibNet.Meeting;

namespace HexServer.Net
{
    public class Session : IEquatable<Session>
    {
        private byte[] _sesId;
        private SessionServer _server;

        public SessionServer Server => _server;

        public Session(byte[] sesId, SessionServer server)
        {
            _sesId = sesId;
            _server = server;
        }

        public bool Equals(Session other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ByteArray.Search(this._sesId, other._sesId) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Session) obj);
        }

        public override int GetHashCode()
        {
            return (_sesId != null ? _sesId.GetHashCode() : 0);
        }

        public static bool operator ==(Session left, Session right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Session left, Session right)
        {
            return !Equals(left, right);
        }
    }
}
