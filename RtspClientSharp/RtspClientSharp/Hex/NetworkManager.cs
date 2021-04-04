using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LibHexCryptoStandard.Hashs;
using LibHexCryptoStandard.Packet;
using LibHexCryptoStandard.Packet.RSA;
using LibHexUtils.Arrays;
using LibHexUtils.Random;
using LibNet.Base;
using LibNet.Meeting;
using LibNet.Meeting.Packets.HexPacket;
using LibNet.Meeting.Parsers;
using LibNet.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities.Net;
using TCP = LibNet.TCP;
using UDP = LibNet.UDP;
using IPAddr = System.Net.IPAddress;

namespace LibRtspClientSharp.Hex
{
    public class NetworkManager
    {
        //control client
        private static TCP.Client _clientControlTcp = null;
        private static UDP.Client _clientDataUdp = null;
        private static System.Net.IPAddress _serverIp = null;

        private static void _connect(ClientBase client, IPAddr ip, int port, byte seconds, byte times)
        {
            byte act = 0;

            do
            {
                try
                {
                    client.Connect(ip, port);
                    break;
                }
                catch (Exception)
                {
                    act++;
                    Thread.Sleep(seconds);
                }
            } while (act < times);

            if (!client.IsConnected())
            {
                //TODO: throw new excpet
            }
        }

        public static void Connect(System.Net.IPAddress ip, int port, byte seconds, byte times)
        {
            if (ip == null) throw new ArgumentNullException(nameof(ip));
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", "Port <= 0");
            }

            _clientControlTcp?.Release();
            _clientControlTcp = new TCP.Client();
            var client = _clientControlTcp;
            
            _connect(_clientControlTcp, ip, port, 2, 5);

            _serverIp = ip;
            var b = HexPacket.Pack(Encoding.UTF8.GetBytes("HMET HI"));

            client.Send(b);
            var s = (TCPState)client.Receive();
            if (!Parser.ParseConConf(s.Buffer))
            {
                //todo: wrong recv
            }

            b = Encoding.UTF8.GetBytes("HMET KEY ");
            var key = CipherManager.GetKeyToSend();
            var keybytes = new byte[b.Length + key.Length];

            Buffer.BlockCopy(b, 0, keybytes, 0, b.Length);
            Buffer.BlockCopy(key, 0, keybytes, b.Length, key.Length);

            b = HexPacket.Pack(keybytes);
            client.Send(b);
            s = (TCPState)client.Receive();
            if (!Parser.ParseKeyConf(s.Buffer))
            {
                //todo: wrong recv
            }
        }

        private static void HostSerMet(string id)
        {
            var idbytes = SHA.SHA3(Encoding.UTF8.GetBytes(id), 256);
            var b = //CipherManager.ProcessByPrivate(
                ByteArray.CopyBytes(0,
                    Encoding.UTF8.GetBytes("HMET HOST "), idbytes);//, 
             //   true, true);

            _clientControlTcp.Send(HexPacket.Pack(b));
            var s = (TCPState)_clientControlTcp.Receive();

            int controlSPort = -1;
            byte[] control = null;
            b = CipherManager.GetControlConDet(s.Buffer);
            if (!Parser.ParseHostServ(b, out controlSPort, out control))
            {
                //todo: wrong recv
            }

            var encrControl = CipherManager.EncryptControl(control);
            _clientDataUdp?.Release();
            _clientDataUdp = new UDP.Client();
            _connect(_clientDataUdp, _serverIp, controlSPort, 1, 5);
            _clientDataUdp.Send(encrControl);
            var us = (UDPState)_clientDataUdp.Receive();

            if (!CipherManager.ParseAndSetKey(us.Buffer))
            {
                //todo: wrong
            }

            _clientDataUdp.Send(CipherManager.SendAESconf());
        }
        

        public static void HostMet(bool useServer, string id, string key)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Value cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Value cannot be null or empty.", nameof(key));

            if (useServer)
            {
                HostSerMet(id);
            }

            CipherManager.InitEncryption(key);
            _clientDataUdp.StartReceivingFromServerMeeting();
        }

        public static IPEndPoint ConnMet(string metID, out byte[] idclient)
        {
            var buff = ByteArray.CopyBytes(0,Encoding.UTF8.GetBytes("HMET CONN "), SHA.SHA3(metID, 256));

            _clientControlTcp.Send(HexPacket.Pack(buff));
            var s = (TCPState)_clientControlTcp.Receive();

            buff = s.Buffer;
            buff = CipherManager.ProcessByPrivate(buff, false, true);

            if (!Parser.ParseJoinConf(buff, out var sesep, out idclient))
            {
                //todo: wrong recv
            }

            return sesep;
        }

        public static void ConnectSession(IPEndPoint ipServer, byte[] control)
        {

        }
    }
}
