using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LibNet.TCP;

namespace LibNet.Utils
{
    public class ClientTCP
    {
        private TcpClient handler;
        private TCPState _tcpState;

        public ClientTCP(TcpClient handler, TCPState tcpState)
        {
            this.handler = handler;
            this._tcpState = tcpState;
        }

        private ManualResetEvent allDone = new ManualResetEvent(false);
        private Action<TcpClient> action;

        //private bool connected = false;
        public void RunListener(Action<TcpClient> act)
        {
            Console.WriteLine("Waiting for a connection...");
            action = act;
            while (true)
            {
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                handler.Client.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    handler.Client);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
                //if (connected) break;
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            Console.WriteLine("New client: " + handler.RemoteEndPoint);
            var t = new TcpClient();
            t.Client = handler;

            action(t);

            // Create the state object. 
            //_tcpState.setSocket(handler);
            //handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            //    new AsyncCallback(ReadCallback), state);
        }

        public TcpClient Handler => handler;

        public TCPState TcpState => _tcpState;

        public NetworkStream Stream => _tcpState.Stream;

        protected bool Equals(ClientTCP other)
        {
            return Equals(handler, other.handler);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientTCP) obj);
        }

        public override int GetHashCode()
        {
            return (handler != null ? handler.GetHashCode() : 0);
        }

        public static bool operator ==(ClientTCP left, ClientTCP right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClientTCP left, ClientTCP right)
        {
            return !Equals(left, right);
        }

        private sealed class HandlerEqualityComparer : IEqualityComparer<ClientTCP>
        {
            public bool Equals(ClientTCP x, ClientTCP y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.handler, y.handler);
            }

            public int GetHashCode(ClientTCP obj)
            {
                return (obj.handler != null ? obj.handler.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<ClientTCP> HandlerComparer { get; } = new HandlerEqualityComparer();
    }
}
