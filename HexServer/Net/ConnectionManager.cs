using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;

namespace HexServer.Net
{
    public class ConnectionManager
    {
        private Dictionary<int, bool> _serverports;
        private IPAddress _mainIP;
        private int _mainPort;

        public void registerPorst(int startPort, int stopPort)
        {
            if (startPort < 1)
            {
                return;
            }

            if (startPort > stopPort)
            {
                int tmp = startPort;
                startPort = stopPort;
                stopPort = tmp;
            }

            var workports = _serverports ?? new Dictionary<int, bool>();

            for (int port = startPort; port <= stopPort; port++)
            {
                if (!workports.ContainsKey(port))
                {
                    workports.Add(port, false);
                }
            }
        }

        public void create(string ip, int port)
        {
            
            if (ip.Equals("0.0.0.0"))
            {
                _mainIP = IPAddress.Any;
            }
            else
            {
                if (!IPAddress.TryParse(ip, out _mainIP))
                {
                    throw new Exception("Cannot parse ip");
                }
            }
            create(_mainIP, port);
        }

        public void create(IPAddress ip, int port)
        {
            _mainIP = ip;
            _mainPort = port;
            create();
        }

        private void create()
        {
            
        }
    }
}
