using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LibHexCryptoStandard.Algoritm;
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
                
                var l = Encoding.UTF8.GetBytes("HMET HOST OK ");
                var cdat = ss.GetAdminConnection();

                byte[] dataToSend = ByteArray.CopyBytes(0, l, cdat);

                ConnectionManager.MainServer.Send(CipherManager.EncryptRSA(ConnectionManager.GetPublicKey(client.Handler.Client.RemoteEndPoint), dataToSend), client);
            }
            else if (Parser.ParseConn(data, out id, out var port))
            {
                Task.Run(() =>
                {
                    var ses = ConnectionManager.isFreeID(id);//todo:is registred
                    var key = ConnectionManager.GetPublicKey(client.Handler.Client.RemoteEndPoint);
                    var toadmin = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET CONN"),
                        ((IPEndPoint)(client.Handler.Client.LocalEndPoint)).Address.GetAddressBytes(), port,
                        RsaOAEP.KeyToBytes(key));
                    
                    var reply = ses.Server.WaitForRequestFromAdmin(ses.Server.SendMessageAdmin(toadmin, true));
                    if (reply?.Length != 0)
                    {
                        ConnectionManager.MainServer.Send(reply, client);
                    }
                });
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
