using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRtspPlayer.Hex.GUI.Exceptions
{
    public class ControllerInitException : Exception
    {
        private readonly ErrorCode _errorCode = ErrorCode.Success;

        public ErrorCode GetErrorCode() => _errorCode;

        public ControllerInitException(ErrorCode errorCode)
        {
            _errorCode = errorCode;
        }

        public ControllerInitException(string message, ErrorCode errorCode) : base(message)
        {
            _errorCode = errorCode;
        }

        public ControllerInitException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException)
        {
            _errorCode = errorCode;
        }

        protected ControllerInitException(SerializationInfo info, StreamingContext context, ErrorCode errorCode) : base(info, context)
        {
            _errorCode = errorCode;
        }
    }
}
