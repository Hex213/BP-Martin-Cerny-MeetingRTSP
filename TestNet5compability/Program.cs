using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LibHexCryptoStandard.Algoritm;
using RtspClientSharp;
using RtspClientSharp.Rtsp;

namespace TestNet5compability
{
    class Program
    {
        static void Main()
        {
            string statickey = "2192B39425BBD08B6E8E61C5D1F1BC9F428FC569FBC6F78C0BC48FCCDB0F42AE";
            string staticiv = "E1E592E87225847C11D948684F3B070D";
            LibHexCryptoStandard.Algoritm.AesGcm256.init(statickey, staticiv);

            var serverUri = new Uri("rtsp://127.0.0.1:8554/live");
            var credentials = new NetworkCredential("admin", "123456");

            var connectionParameters = new ConnectionParameters(serverUri, credentials);
            var cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Press any key to connect");
            Console.ReadLine();

            connectionParameters.CancelTimeout = TimeSpan.FromSeconds(30);
            connectionParameters.ConnectTimeout = TimeSpan.FromSeconds(30);
            connectionParameters.ReceiveTimeout = TimeSpan.FromSeconds(30);
            /*
             * Zo dna 7.3. - (0,f,U), (0,f,U), (0,t,T), (0,f,T), (1,f,U), (1,f,U), (1,t,T), (1,f,T) 
             * Enrypt:  (0,1)
             * Base64:  (f,t)
             * Protoc:  (U,T)
             * Pass: (1,f,T), (1,f,U)
             * Fail: (1,t,T), (1,t,U), (0,t,U), (0,f,U), (0,t,T), (0,f,T)
             */
            connectionParameters.Enryption = false;
            connectionParameters.UseBase64 = false;
            connectionParameters.RtpTransport = RtpTransportProtocol.TCP;

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

                        Console.WriteLine("Connected.");

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