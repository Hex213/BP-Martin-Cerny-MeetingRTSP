using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LibNet.Utils;

namespace LibNet.Base
{
    public abstract class ClientBase
    {
        public abstract void Connect(IPAddress ip, int port);
        public abstract bool IsConnected();
        public abstract void Send(byte[] bytes);
        public abstract object Receive();
        public abstract void Release();
    } 
}
