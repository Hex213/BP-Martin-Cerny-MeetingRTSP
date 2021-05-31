using System;
using System.Runtime.Serialization;

namespace LibRtspClientSharp.Hex.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException()
        {
        }

        protected ConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ConnectionException(string message) : base(message)
        {
        }

        public ConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
