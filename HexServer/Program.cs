using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HexServer.Net;
using UDP = LibNet.UDP;
using TCP = LibNet.TCP;

namespace HexServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("-----SERVER-----");
                ConnectionManager.RegisterPorts(40001, 40010);
                ConnectionManager.Create("127.0.0.1", 40000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            Console.ReadKey();
        }
    }
}
