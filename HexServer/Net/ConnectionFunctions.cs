using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LibHexUtils.Arrays;
using LibHexUtils.Random;
using LibNet.Base;
using LibNet.Meeting.Packets.Exceptions;
using LibNet.Meeting.Packets.HexPacket;
using LibNet.Meeting.Parsers;
using LibNet.Utils;

namespace HexServer.Net
{
    public class ConnectionFunctions : AdditionalFunctions
    {
        private bool ParseHost(byte[] hpkt, ClientTCP client, out byte[] id)
        {
            try
            {
                //var key = ConnectionManager.GetPublicKey(client.Handler.Client.RemoteEndPoint);
                //var decr = CipherManager.DecryptRSA(key, hpkt);
                return Parser.ParseID(hpkt/*decr*/, out id, "HMET HOST ", /*false*/true);
            }
            catch (Exception)
            {
                id = null;
                return false;
            }
        }

        private void Parse(byte[] data, ClientTCP client)
        {
            byte[] id = null;
            if (Parser.ParseCon(data))
            {
                ConnectionManager.MainServer.Send(HexPacket.Pack(Encoding.UTF8.GetBytes("HMET OK")), client);
            }
            else if (Parser.ParseKey(data, out var key))
            {
                ConnectionManager.RegisterPublicKey(client.Handler.Client.RemoteEndPoint, key);
                ConnectionManager.MainServer.Send(HexPacket.Pack(Encoding.UTF8.GetBytes("HMET KEY OK")), client);
            }
            else if(ParseHost(data, client, out id))
            {
                var ss = ConnectionManager.CreateSession(id, client.Handler.Client.RemoteEndPoint);
                var adminPortBytes = BitConverter.GetBytes(ss.MainPort);
                var control = ss.Control;
                
                var l = Encoding.UTF8.GetBytes("HMET HOST OK ");

                byte[] dataToSend = ByteArray.CopyBytes(0, l, adminPortBytes, control, id);// new byte[l.Length + adminPortBytes.Length + control.Length];
                //Buffer.BlockCopy(l, 0, dataToSend, 0, l.Length);
                //Buffer.BlockCopy(adminPortBytes, 0, dataToSend, l.Length, 4);
                //Buffer.BlockCopy(control, 0, dataToSend, l.Length+4, control.Length);
                
                ConnectionManager.MainServer.Send(CipherManager.EncryptRSA(ConnectionManager.GetPublicKey(client.Handler.Client.RemoteEndPoint), dataToSend), client);
            }
            else if (Parser.ParseID(data, out id))
            {
                var ses = ConnectionManager.isFreeID(id);
                byte[] dataToSend = null;
                if (ses != null)
                {
                    dataToSend = ByteArray.CopyBytes(0,
                        Encoding.UTF8.GetBytes("HMET CONN OK "),
                        ConnectionManager.MainIp.GetAddressBytes(), BitConverter.GetBytes(ses.Server.MainPort),
                        HexRandom.GetRandomBytes(16));
                }
                else
                {
                    dataToSend = ByteArray.CopyBytes(0,
                        Encoding.UTF8.GetBytes("HMET CONN OK "),
                        new byte[] {0, 0, 0, 0}, new byte[] {0, 0, 0, 0},
                        HexRandom.GetRandomBytes(16));
                }

                ConnectionManager.MainServer.Send(CipherManager.EncryptRSA(ConnectionManager.GetPublicKey(client.Handler.Client.RemoteEndPoint), dataToSend), client);
            }

            Task.Run(() => ConnectionManager.MainServer.ReadTask(client));
        }

        public override bool RecvCondition(object args)
        {
            ClientTCP workClient = (ClientTCP)args;
            var data = new byte[workClient.TcpState.totalRead];
            Buffer.BlockCopy(workClient.TcpState.Buffer, 0, data, 0, workClient.TcpState.totalRead);
            try
            {
                HexPacket.CheckPacket(data, 0, out var tmp0, out var tmp1);
                return true;
            }
            catch (PacketException)
            {
                return false;
            }
        }

        public override void RecvFunc(object args)
        {
            byte[] data;

            ClientTCP workClient = (ClientTCP)args;
            data = new byte[workClient.TcpState.totalRead];
            Buffer.BlockCopy(workClient.TcpState.Buffer, 0, data, 0, workClient.TcpState.totalRead);
            Parse(data, workClient);
        }

        public override bool SendCondition(object args)
        {
            throw new NotImplementedException();
        }

        public override void SendFunc(object args)
        {
            throw new NotImplementedException();
        }
    }
}
