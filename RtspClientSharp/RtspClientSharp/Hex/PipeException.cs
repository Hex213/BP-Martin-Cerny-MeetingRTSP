using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LibRtspClientSharp.Hex
{
    public class PipeException : Exception
    {
        public int Error_code { get; } = 0;

        public PipeException(int errorCode)
        {
            this.Error_code = errorCode;
        }

        protected PipeException(SerializationInfo info, StreamingContext context, int errorCode) : base(info, context)
        {
            this.Error_code = errorCode;
        }

        public PipeException(string message, int errorCode) : base(message)
        {
            this.Error_code = errorCode;
        }

        public PipeException(string message, Exception innerException, int errorCode) : base(message, innerException)
        {
            this.Error_code = errorCode;
        }
    }
}
