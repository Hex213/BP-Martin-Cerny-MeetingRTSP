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
            var s = new UDP.Server(IPAddress.Parse("127.0.0.1"), 44000);
            var st = new TCP.Server(IPAddress.Parse("127.0.0.1"), 44001);

            Task.Run(() => s.StartMainServer());
            Task.Run(() => st.StartMainServer());

            //Thread.Sleep(1000);
            Console.WriteLine("--");
            Task.Run(() => { 
                var c = new UDP.Client(); 
                c.Connect(IPAddress.Parse("127.0.0.1"), 44000);
                c.SendTest();
                c.Receive();
            });
            //Thread.Sleep(1000);
            //Console.WriteLine("--");
            //Task.Run(() => {
            //    var c = new UDP.Client();
            //    c.Connect(IPAddress.Parse("127.0.0.1"), 44000);
            //    c.SendTest();
            //    c.Receive();
            //});
            //Thread.Sleep(1000);
            Console.WriteLine("--");
            Task.Run(() => {
                var c = new TCP.Client();
                c.Connect(IPAddress.Parse("127.0.0.1"), 44001);
                c.SendTest();
                c.Receive();
            });
            //Thread.Sleep(TimeSpan.FromMinutes(2));
            //Console.WriteLine("--");
            //Task.Run(() => {
            //    var c = new TCP.Client();
            //    c.Connect(IPAddress.Parse("127.0.0.1"), 44001);
            //    c.SendTest();
            //    c.Receive();
            //});

            Console.WriteLine("--");

            Console.ReadKey();
        }
    }
}
