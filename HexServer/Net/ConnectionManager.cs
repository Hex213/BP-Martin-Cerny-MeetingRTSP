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
using LibHexCryptoStandard.Algoritm;
using LibNet.Meeting;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using TCP = LibNet.TCP;

namespace HexServer.Net
{
    public static class ConnectionManager
    {
        private static IPAddress _mainIP;

        public static IPAddress MainIp => _mainIP;

        private static int _mainPort;
        private static Dictionary<int, bool> _serverports;

        private static Dictionary<EndPoint, RsaKeyParameters> clientsPublicKeys;

        private static List<Session> _sessions;

        public static TCP.Server MainServer { get; private set; }

        public static Session isFreeID(byte[] id)
        {
            var tmp = new Session(id, null);
            return _sessions.Find(x => x.Equals(tmp));
        }

        private static void CheckPorts()
        {
            if (_serverports == null)
            {
                _serverports ??= new Dictionary<int, bool>();
                return;
            }

            foreach (var pair in _serverports)
            {
                var isUsed = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveUdpListeners()
                    .Any(p => p.Port == pair.Key);
                if (isUsed)
                {
                    _serverports[pair.Key] = isUsed;
                }
            }
        }

        public static void RegisterPublicKey(EndPoint client, byte[] key)
        {
            var k = CipherManager.GetKey(key);
            if (!clientsPublicKeys.ContainsKey(client))
            {
                clientsPublicKeys.Add(client, k);
            }
            else
            {
                clientsPublicKeys[client] = k;
            }
        }

        public static RsaKeyParameters GetPublicKey(EndPoint client)
        {
            if (clientsPublicKeys.TryGetValue(client, out var key))
            {
                return key;
            }
            else
            {
                throw new IndexOutOfRangeException("Client has no public key!");
            }
        }

        public static SessionServer CreateSession(byte[] id, EndPoint adminEndPoint)
        {
            CheckPorts();

            int port = 0;
            foreach (var pair in _serverports.Where(pair => pair.Value == false))
            {
                port = pair.Key;
                _serverports[port] = true;
                break;
            }

            if (port == 0)
            {
                //todo: throw
            }

            var ss = new SessionServer(_mainIP, port, CipherManager.GetRandBytes(32), adminEndPoint, GetPublicKey(adminEndPoint));
            var sesion = new Session(id, ss);
            if (_sessions.Contains(sesion))
            {
                _serverports[port] = false;
                //todo: throw
            }

            _sessions.Add(sesion);
            Task.Run(() => ss.StartServer());

            return ss;
        }

        public static void RegisterPorts(int startPort, int stopPort)
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

            _serverports = workports;
        }

        public static void Create(string ip, int port)
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
            Create(_mainIP, port);
        }

        public static void Create(IPAddress ip, int port)
        {
            _mainIP = ip;
            _mainPort = port;
            Create();
        }

        private static void Create()
        {
            MainServer = new TCP.Server(_mainIP, _mainPort, new ConnectionFunctions());
            clientsPublicKeys = new Dictionary<EndPoint, RsaKeyParameters>();
            _sessions = new List<Session>();
            Task.Run(() => MainServer.StartServer());
        }
    }
}
