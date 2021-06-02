using System;
using System.Collections.Generic;
using System.Text;

namespace LibNet.Meeting.Parsers
{
    public enum OptType
    {
        None,
        Port,
        PortServ,
        Unknown
    };

    public class OptionReply
    {
        public OptType Type { get; set; }
        public object[] AData { get; set; }
    }
}
