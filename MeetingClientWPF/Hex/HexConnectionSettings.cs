using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeetingClientWPF.Hex
{
    public static class HexConnectionSettings
    {
        private static string _id;
        private static IPAddress _ip;
        private static bool _isInit = false;

        public static string Id
        {
            get => _id;
            set
            {
                if (_isInit) return;
                _id = value;
            }
        }

        public static IPAddress Ip
        {
            get => _ip;
            set
            {
                if(_isInit) return;
                _ip = value;
            }
        }

        public static void Init()
        {
            _isInit = true;
        }
    }
}
