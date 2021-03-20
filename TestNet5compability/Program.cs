using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibHexCryptoStandard.Algoritm;
using LibHexCryptoStandard.Packet;
using LibHexCryptoStandard.Packet.AES;
using LibHexCryptoStandard.Packet.RSA;
using Org.BouncyCastle.Crypto.Parameters;
using RtspClientSharp;
using RtspClientSharp.Rtsp;

namespace TestNet5compability
{
    class Program
    {
        static void Main()
        {
            //init
            AesGcm256.init("test");
            var aeskey = AesGcm256.Key;
            var rsakey = RsaOAEP.GenerateKeyPair(2048);

            //encrpt data
            var data = Encoding.UTF8.GetBytes("Test");
            var hpdata = new HexPacketAES(data, false, EncryptType.Encrypt);
            var pkt = (byte[])hpdata.Encrypt();

            //test rsa send key
            var t = HexPacketRSA.GetKeyToPacket((RsaKeyParameters) rsakey.Public);
            var r = HexPacketRSA.GetKeyFromPacket(t);
            //Debug.Assert(((RsaKeyParameters)rsakey.Public).Equals(r), "Diff rsa key");

            //test aes send key
            var rs = new HexPacketRSA(aeskey);
            var kdata = rs.Encrypt((RsaKeyParameters) rsakey.Private);
            var rs1 = new HexPacketRSA(kdata);
            var dkdata = rs1.Decrypt(r, true);
            
            //Debug.Assert(aeskey.Equals(dkdata), "Diff aes key");

            //decrypt data
            var hpktd = new HexPacketAES(pkt, false, EncryptType.DecryptPacket);
            var dd = hpktd.Decrypt();
            //Debug.Assert(dd.Equals("Test"), "Bad message");

            string s = "";
            HexPacket h = new HexPacket();
            Object oj = h as object;
            Object o = s as object;
            Console.WriteLine(o.GetType());
            Console.WriteLine(oj.GetType());

            //string statickey = "2192B39425BBD08B6E8E61C5D1F1BC9F428FC569FBC6F78C0BC48FCCDB0F42AE";
            //string staticiv = "E1E592E87225847C11D948684F3B070D";
            //LibHexCryptoStandard.Algoritm.AesGcm256.init(statickey, staticiv);

            //var serverUri = new Uri("rtsp://127.0.0.1:8554/live");
            //var credentials = new NetworkCredential("admin", "123456");

            //var connectionParameters = new ConnectionParameters(serverUri, credentials);
            //var cancellationTokenSource = new CancellationTokenSource();

            ///*test passage*/
            //const bool test = true;
            //connectionParameters.Enryption = test;
            //connectionParameters.UseBase64 = test;
            //connectionParameters.RtpTransport = RtpTransportProtocol.TCP;

            //Console.WriteLine("(ENCRYPT_PKT)=" + connectionParameters.Enryption + ", (ENCRYPT_USEBASE64)=" +
            //                  connectionParameters.UseBase64 + ", (PROTOCOL)=" +
            //                  connectionParameters.RtpTransport);

            //byte[] testBytes = { 15, 32, 14, 205, 0};
            //var hpkt = HexPacketAES.CreatePacketToEncrypt(testBytes, connectionParameters.UseBase64);
            //byte[] pkt = (byte[]) hpkt.Encrypt();

            //var keys = HexPacketRSA.GenerateKeyPair(2048);
            //var enc = HexPacketRSA.Encrypt(testBytes, (RsaKeyParameters) keys.Private);
            //var dec = HexPacketRSA.Decrypt(enc, (RsaKeyParameters) keys.Public);
            ///*end passage*/

            //connectionParameters.CancelTimeout = TimeSpan.FromSeconds(30);
            //connectionParameters.ConnectTimeout = TimeSpan.FromSeconds(30);
            //connectionParameters.ReceiveTimeout = TimeSpan.FromSeconds(30);

            //Console.WriteLine("Press any key to connect");
            //Console.ReadLine();

            //Task connectTask = ConnectAsync(connectionParameters, cancellationTokenSource.Token);

            //Console.WriteLine("Press any key to cancel");
            //Console.ReadLine();

            //cancellationTokenSource.Cancel();

            //Console.WriteLine("Canceling");
            //connectTask.Wait(CancellationToken.None);
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