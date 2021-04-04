using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LibRtspClientSharp.Hex;
using RtspClientSharp;
using RtspClientSharp.Rtsp;

namespace TestNet5compability2
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(5000);
            Console.WriteLine("-----CLIENT connect-----");
            CipherManager.NewID();
            NetworkManager.Connect(IPAddress.Parse("127.0.0.1"), 40000, 2, 5);
            var ipSes = NetworkManager.ConnMet("live", out var tempID);
            var serverUri = new Uri("rtsp://"+ipSes.Address+":"+ipSes.Port);
            var credentials = new NetworkCredential("admin", "123456");

            var connectionParameters = new ConnectionParameters(serverUri, credentials);
            var cancellationTokenSource = new CancellationTokenSource();
            connectionParameters.Enryption = false;
            connectionParameters.UseBase64 = false;
            connectionParameters.UseBase64 = true;
            connectionParameters.RtpTransport = RtpTransportProtocol.UDP;

            Task connectTask = ConnectAsync(connectionParameters, cancellationTokenSource.Token);

            Console.WriteLine("Press any key to cancel");
            Console.ReadLine();

            cancellationTokenSource.Cancel();

            Console.WriteLine("Canceling");
            connectTask.Wait(CancellationToken.None);
        }

        private static async Task ConnectAsync(ConnectionParameters connectionParameters, CancellationToken token)
        {
            try
            {
                if (connectionParameters.UseServer)
                {
                    connectionParameters.RtpTransport = RtpTransportProtocol.UDP;
                }

                TimeSpan delay = TimeSpan.FromSeconds(5);

                using (var rtspClient = new RtspClient(connectionParameters))
                {
                    rtspClient.FrameReceived +=
                        (sender, frame) =>
                        {
                            Console.WriteLine(
                                $"New frame {frame.Timestamp}: {frame.GetType().Name} - {frame.FrameSegment.Count}");
                        };

                    while (true)
                    {
                        Console.WriteLine("Connecting...");

                        try
                        {
                            await rtspClient.ConnectAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                            continue;
                        }

                        Console.WriteLine("IsConnected.");

                        try
                        {
                            await rtspClient.ReceiveAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
