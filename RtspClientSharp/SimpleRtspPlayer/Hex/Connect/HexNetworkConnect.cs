using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RtspClientSharp;
using SimpleRtspPlayer.Hex.Globals;
using SimpleRtspPlayer.Hex.GUI.Exceptions;
using SimpleRtspPlayer.Properties;
using static SimpleRtspPlayer.Hex.Connect.Check;

namespace SimpleRtspPlayer.Hex.Connect
{
    internal enum Check
    {
        Name = 1,
        Id = 2,
        Paddr = 4,
        Pport = 8,
        Puri = 16,
        Credint = 32,
        
    };

    public static class HexNetworkConnect
    {
        private static byte _total = 0;
        private static string _id;
        private static Uri _ipUri;
        private static IPAddress _ipAddr;
        private static ConnectionParameters _conParams;
        private static NetworkCredential _networkCredential;
        private static string _name;
        private static ushort _port;

        public static string Id => _id;
        public static IPAddress IpAddr => _ipAddr;
        public static string Name => _networkCredential.UserName;

        public static bool AddName(string name)
        {
            bool cor;

            // ReSharper disable once AssignmentInConditionalExpression
            _total = (cor = !(name.Length < 3))
                ? (byte)(_total | (byte)Connect.Check.Name)
                : (byte)(_total & (~(byte)Connect.Check.Name));

            if (cor) _name = name;

            return cor;
        }

        public static bool AddId(string id)
        {
            bool cor;
            // ReSharper disable once AssignmentInConditionalExpression
            _total = (cor = (id.Length >= Globals.NetworkGlobal.MinIdLength))
                ? (byte)(_total | (byte)Connect.Check.Id)
                : (byte)(_total & (~(byte)Connect.Check.Id));

            if (cor)
            {
                _id = id;
            }

            return cor;
        }

        public static bool AddPort(string port)
        {
            var cor = ushort.TryParse(port, out _port);

            // ReSharper disable once AssignmentInConditionalExpression
            _total = cor
                ? (byte)(_total | (byte)Connect.Check.Pport)
                : (byte)(_total & (~(byte)Connect.Check.Pport));

            return cor;
        }

        public static bool AddIp(string ip)
        {
            bool cor;
            var uri = !ip.StartsWith(NetworkGlobal.RtspPrefix) && !ip.StartsWith(NetworkGlobal.HttpPrefix) ?
                NetworkGlobal.RtspPrefix + ip : ip;

            // ReSharper disable once AssignmentInConditionalExpression
            _total = (cor = Uri.TryCreate(uri, UriKind.Absolute, out _ipUri))
                ? (byte) (_total | (byte) Connect.Check.Puri)
                : (byte) (_total & (~(byte) Connect.Check.Puri));

            if (cor)
            {
                // ReSharper disable once AssignmentInConditionalExpression
                _total = (cor = IPAddress.TryParse(ip, out _ipAddr))
                    ? (byte) (_total | (byte) Connect.Check.Paddr)
                    : (byte) (_total & (~(byte) Connect.Check.Paddr));
            }

            return cor;
        }

        public static bool AddCredential(SecureString password)
        {
            if ((_total & (byte) Connect.Check.Name) == 0) return false;

            _total = (_total & (byte)Connect.Check.Name) != 0
                ? (byte)(_total | (byte)Connect.Check.Credint)
                : (byte)(_total & (~(byte)Connect.Check.Credint));

            _networkCredential ??= new NetworkCredential(_name, password);
            return true;
        }

        public static class EnumUtil
        {
            public static IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }

        public static bool Check()
        {
            var check = EnumUtil.GetValues<Check>().Aggregate<Check, byte>(0, (current, chck) => (byte) (current + (byte) chck));
            return check == _total;
        }

        private static void _correctUri()
        {
            var ipUri =_ipUri.AbsoluteUri;
            if (!ipUri.StartsWith(NetworkGlobal.RtspPrefix) && !ipUri.StartsWith(NetworkGlobal.HttpPrefix))
            {
                ipUri = NetworkGlobal.RtspPrefix + ipUri;
            }

            if (!ipUri.EndsWith(HexNetworkConnect.Id))
            {
                ipUri = ipUri + (ipUri[ipUri.Length - 1] != '/' ? "/" : "") + HexNetworkConnect.Id;
            }

            if (_ipUri.Port <= 0)
            {
                ipUri = ipUri.Insert(ipUri.IndexOf(_ipUri.IdnHost, StringComparison.Ordinal) + _ipUri.IdnHost.Length, ":" + _port);
            }

            if (!Uri.TryCreate(ipUri, UriKind.Absolute, out _ipUri))
            {
                throw new NetworkControllerException("Cannot create uri from address: " + ipUri, ErrorCode.UriCreate);
            }
        }

        public static ConnectionParameters GetConnectionParameters(bool renew)
        {
            if (!Check()) return null;

            _correctUri();

            if (renew) _conParams = null;
            _conParams ??= new ConnectionParameters(_ipUri, _networkCredential);
            _conParams.RtpTransport = Settings.Default.TypeOfProtocol ? RtpTransportProtocol.TCP : RtpTransportProtocol.UDP;
            _conParams.CancelTimeout = TimeSpan.FromMilliseconds(Settings.Default.CancelTime);

            return _conParams;
        }

        public static double Ping()
        {
            double pingms = -1;

            if ((_total & (byte) Connect.Check.Paddr) != 0)
            {
                //TODO: ping
            }

            return pingms;
        }
    }
}
