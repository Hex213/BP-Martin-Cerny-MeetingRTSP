using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibNet.Utils
{
    public static class PrintNet
    {
        public static void printRead(EndPoint local, EndPoint remote, int readed)
        {
            Console.WriteLine("R " + local + "<-(" + readed + ")-" + remote);
        }
        public static void printRead(Socket s, int readed)
        {
            Console.WriteLine("R " + s.LocalEndPoint + "<-(" + readed + ")-" + s.RemoteEndPoint);
        }
        public static void printSend(Socket s, int send)
        {
            Console.WriteLine("S " + s.LocalEndPoint + "-(" + send + ")->" + s.RemoteEndPoint);
        }
        public static void printSend(EndPoint local, EndPoint remote, int send)
        {
            Console.WriteLine("S " + local + "-(" + send + ")->" + remote);
        }
    }
}
