﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LibHexUtils.Arrays;
using LibNet.Base;
using LibNet.Utils;

namespace LibNet.UDP
{
    public class Client : ClientBase
    {
        private IPEndPoint _serverEndPoint;
        private UdpClient client;

        public bool messageSent = false;
        public bool messageReceived = false;
        private bool started = false;

        public Client()
        {
            client = new UdpClient();
        }

        public override void Connect(IPAddress ip, int port)
        {
            _serverEndPoint = new IPEndPoint(ip, port);
            client.Connect(_serverEndPoint);
        }

        public override bool IsConnected()
        {
            try
            {
                Send(new byte[] { 103, 111, 100 });
                return messageSent;
            }
            catch (ObjectDisposedException )
            {
                return false;
            }
        }

        public override void Send(byte[] bytes)
        {
            messageSent = false;
            byte[] sendBytes = bytes;

            int sended = -1;

            try
            {
                sended = client.Send(sendBytes, sendBytes.Length);
                if (sended == bytes.Length) messageSent = true;
                do
                {
                    var tS = ByteArray.SubArray(sendBytes, sended);
                    sended += client.Send(tS, tS.Length);
                    if (sended >= bytes.Length) messageSent = true;
                } while (!messageSent);
            }
            catch (Exception)
            {
                // ignored
            }

            PrintNet.printSend(client.Client, sended);
        }

        public void SendCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)ar.AsyncState;

            Console.WriteLine($"number of bytes sent: {u.EndSend(ar)}");
            messageSent = true;
        }

        public override object Receive()
        {
            // Receive a message and write it to the console.
            messageReceived = false;
            IPEndPoint e = _serverEndPoint;

            var s = new UDPState();
            s.e = e;
            s.u = client;
            
            client.BeginReceive(new AsyncCallback(ReceiveCallback), s);

            // Do some work while we wait for a message. For this example, we'll just sleep
            while (!messageReceived)
            {
                Thread.Sleep(100);
            }

            return s;
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            var state = (UDPState) (ar.AsyncState);
            UdpClient u = state.u;
            IPEndPoint e = state.e;

            state.buffer = u.EndReceive(ar, ref e);

            PrintNet.printRead(u.Client.LocalEndPoint, e, state.Buffer.Length);
            state.e = e; 
            
            messageReceived = true;
        }

        public override void Release()
        {
            throw new NotImplementedException();
        }
    }
}
