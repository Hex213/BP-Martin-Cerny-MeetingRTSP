using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibNet.UDP
{
    public class Server
    {
        private readonly int mainPort;
        private IPAddress mainIP;

        public int MainPort => mainPort;

        public IPAddress MainIp => mainIP;

        public Server(IPAddress mainIp, int mainPort)
        {
            this.mainPort = mainPort;
            mainIP = mainIp;
        }

        public async Task StartMainServer()
        {
            UdpClient listener = new UdpClient(mainPort);
            IPEndPoint groupEP = new IPEndPoint(mainIP, mainPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
