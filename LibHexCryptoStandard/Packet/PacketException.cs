using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LibHexCryptoStandard.Packet
{
    public class PacketException : Exception
    {
        public PacketException()
        {
        }

        protected PacketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PacketException(string message) : base(message)
        {
        }

        public PacketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
