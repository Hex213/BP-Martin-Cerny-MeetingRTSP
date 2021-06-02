using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LibHexUtils.Arrays;
using LibHexUtils.Register;
using LibNet.Utils;

namespace LibNet.TCP
{
    public class Proxy : Base.Proxy
    {
        private TcpClient _clCenter;
        private TcpClient _centerCl;

        public Proxy(IPEndPoint ipSrc, IPEndPoint ipDes, bool srcToProxy, bool desToProxy, bool consoleOutput = false) : base(ipSrc, ipDes)
        {
            _clCenter = new TcpClient();
            _clCenter.Connect(_ipSrc);
            _centerCl = new TcpClient();
            _centerCl.Connect(_ipDes);
            if (srcToProxy) register.AddFlag(1);
            if (desToProxy) register.AddFlag(1 << 1);
            register.AddFlag(1 << 2);
        }

        public Proxy(IPEndPoint ipSrc, IPEndPoint ipDes, Socket srcProx, Socket proxDes, bool srcToProxy, bool desToProxy, bool connect = true, bool consoleOutput = false) : base(ipSrc, ipDes)
        {
            _clCenter = new TcpClient();
            _clCenter.Client = srcProx;
            if (connect) _clCenter.Client.Connect(_ipSrc);
            _centerCl = new TcpClient();
            _centerCl.Client = proxDes;
            if (connect) _centerCl.Client.Connect(_ipDes);
            if (srcToProxy) register.AddFlag(1);
            if (desToProxy) register.AddFlag(1 << 1);
            register.AddFlag(1 << 2);
        }

        private TcpClient _createTcpClient(IPEndPoint ipEnd, bool connect)
        {
            if (ipEnd == null)
            {
                throw new Exception("Proxy with no endpoint.");
            }
            var c = new TcpClient();
            if (connect)
            {
                c.Connect(ipEnd);
            }
            return c;
        }

        public Proxy(TcpClient srcProx, TcpClient proxDes, bool srcToProxy, bool desToProxy, IPEndPoint ipSrc = null, IPEndPoint ipDes = null, bool connect = true, bool consoleOutput = false) 
            : base(srcProx == null ? ipSrc : (IPEndPoint) srcProx.Client.LocalEndPoint,
                proxDes == null ? ipDes : (IPEndPoint)proxDes.Client.LocalEndPoint)
        {
            if (srcProx == null && ipSrc != null)
            {
                srcProx = _createTcpClient(ipSrc, connect);
            }
            else
            {
                throw new Exception("Proxy with no endpoint.");
            }
            if (proxDes == null && _ipDes != null)
            {
                proxDes = _createTcpClient(_ipDes, connect);
            }
            else
            {
                throw new Exception("Proxy with no endpoint.");
            }
            
            _clCenter = srcProx;
            _centerCl = proxDes;
            if (srcToProxy) register.AddFlag(1);
            if (desToProxy) register.AddFlag(1 << 1);
            register.AddFlag(1 << 2);
        }

        public override void StopProxy()
        {
            register.RemFlag(1 << 4);
            register.AddFlag(1 << 3);
        }

        public override void StartProxy()
        {
            if (register.IsPresent(1 << 4))
            {
                return;
            }

            register.AddFlag(1 << 4);

            var bufferSA = new byte[2048];
            
            if (register.IsPresent(1)) Task.Run(() =>
            {
                var bufferCS = new byte[2048];
                while (!register.IsPresent(1 << 3))
                {
                    try
                    {
                        var r = _clCenter.Client
                            .Receive(bufferCS); //stream_cs.ReadAsync(bufferCS, 0, bufferCS.Length);
                        if (r > 0)
                        {
                            if (!register.IsPresent(1 << 3))
                            {
                                Console.Write("PX(C->P) ");
                                PrintNet.printRead(_clCenter.Client.LocalEndPoint, _clCenter.Client.RemoteEndPoint, r);
                            }
                            var s = _centerCl.Client.Send(bufferCS, 0, r, SocketFlags.None);
                            if (!register.IsPresent(1 << 3))
                            {
                                Console.Write("PX(P->D) ");
                                PrintNet.printSend(_centerCl.Client.LocalEndPoint, _centerCl.Client.RemoteEndPoint, s);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            });
            if (register.IsPresent(1)) Task.Run(() =>
            {
                var bufferCS = new byte[2048];
                while (!register.IsPresent(1 << 3))
                {
                    try
                    {
                        var r = _centerCl.Client
                            .Receive(bufferCS); //stream_cs.ReadAsync(bufferCS, 0, bufferCS.Length);
                        if (r > 0)
                        {
                            if (!register.IsPresent(1 << 3))
                            {
                                Console.Write("PX(D->P) ");
                                PrintNet.printRead(_centerCl.Client.LocalEndPoint, _centerCl.Client.RemoteEndPoint, r);
                            }
                            var s = _clCenter.Client.Send(bufferCS, 0, r, SocketFlags.None);
                            if (!register.IsPresent(1 << 3))
                            {
                                Console.Write("PX(P->C) ");
                                PrintNet.printSend(_clCenter.Client.LocalEndPoint, _clCenter.Client.RemoteEndPoint, s);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}
