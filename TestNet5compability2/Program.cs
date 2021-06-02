using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LibRtspClientSharp.Hex;
using LibRtspClientSharp.Hex.Exceptions;
using RtspClientSharp;
using RtspClientSharp.RawFrames;
using RtspClientSharp.Rtsp;

namespace TestNet5compability2
{
    class Program
    {
        private static bool enable = true;

        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            Console.WriteLine("-----CLIENT connect-----");
            CipherManager.NewID();
            NetworkManager.InitConParams(new ConnectionParameters(new Uri("rtsp://127.0.0.1:1/"))
            {
                Enryption = enable,
                UseBase64 = !enable,
                UseServer = !enable,
                ReceiveTimeout = TimeSpan.FromSeconds(10),
                CancelTimeout = TimeSpan.FromSeconds(10),
                RtpTransport = RtpTransportProtocol.UDP
            });
            do
            {
                try
                {
                   // Console.ReadKey();
                    NetworkManager.ConnectBase(IPAddress.Parse("127.0.0.1"), 40000, 2, 5);
                    break;
                }
                catch (ConnectionException)
                {
                    do
                    {
                        Console.WriteLine("Cannot connect to server!\nDo you want to try to connect again?\n(y/n): ");
                        var line = Console.ReadLine();
                        bool BREAK = false;
                        if (line != null)
                        {
                            line = line.ToLower();
                            switch (line[0])
                            {
                                case 'y':
                                    BREAK = true;
                                    break;
                                case 'n':
                                    return;
                            }
                        }

                        if (!BREAK) continue;
                        BREAK = false;
                        break;
                    } while (true);
                }
            } while (true);
            
            Console.WriteLine("Press enter to join the meeting!");
            Console.ReadKey();
            string suffix = "live";
            var ipSes = NetworkManager.ConnMet(suffix);
            var serverUri = new Uri("rtsp://"+ipSes.Address+":"+ipSes.Port+"/"+suffix);
            //var credentials = new NetworkCredential("admin", "123456");
            var cancellationTokenSource = new CancellationTokenSource();
            NetworkManager.updateUri(serverUri);
            var connectionParameters = NetworkManager.ConnectionParameters;/*new ConnectionParameters(serverUri, credentials)
            {
                Enryption = enable,
                UseBase64 = !enable,
                UseServer = enable,
                RtpTransport = RtpTransportProtocol.UDP
            };*/
            
            Task connectTask = ConnectAsync(connectionParameters, cancellationTokenSource.Token);

            Console.WriteLine("Press any key to cancel");
            Console.ReadLine();

            cancellationTokenSource.Cancel();

            Console.WriteLine("Canceling");
            connectTask.Wait(CancellationToken.None);
        }

        private static async Task ConnectAsync(ConnectionParameters connectionParameters, CancellationToken token)
        {
            //ulong c = 0;
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
                            //if(frame.Type == FrameType.Video)
                            //c += (ulong) frame.FrameSegment.Count;
                            Console.WriteLine(
                                $"New frame {frame.Timestamp}: {frame.GetType().Name} - {frame.FrameSegment.Count}");
                        };

                    while (true)
                    {
                        Console.WriteLine("Connecting...");

                        try
                        {
                            NetworkManager.InitConParams(connectionParameters);
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
