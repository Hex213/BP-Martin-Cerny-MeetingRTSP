using System;
using System.Runtime.Serialization;

namespace SimpleRtspPlayer.Hex.Engine.GUI.Exceptions
{
    public class NetworkControllerException : Exception
    {
        private ErrorCode _errorCode = ErrorCode.Success;

        public NetworkControllerException(ErrorCode errorCode)
        {
            _errorCode = errorCode;
        }

        public NetworkControllerException(string message, ErrorCode errorCode) : base(message)
        {
            _errorCode = errorCode;
        }

        public NetworkControllerException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException)
        {
            _errorCode = errorCode;
        }

        protected NetworkControllerException(SerializationInfo info, StreamingContext context, ErrorCode errorCode) : base(info, context)
        {
            _errorCode = errorCode;
        }
    }
}
