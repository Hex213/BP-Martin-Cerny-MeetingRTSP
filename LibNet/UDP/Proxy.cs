using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LibHexUtils.Arrays;
using LibHexUtils.Register;
using LibNet.Utils;

namespace LibNet.UDP
{
    public class Proxy : Base.Proxy
    {
        private UdpClient _clCenter;
        private UdpClient _centerCl;

        // Register info
        // [Running][ExitProxy][ConsOutput][desToProxy][srcToProxy]LSB
        private Register register = new Register();

        public Proxy(bool srcToProxy, bool desToProxy, bool consoleOutput = false)
        {
            if (srcToProxy) register.AddFlag(1);
            if (desToProxy) register.AddFlag(1 << 1);
            if (consoleOutput) register.AddFlag(1 << 2);
        }

        public Proxy(IPEndPoint ipSrc, IPEndPoint ipDes, bool srcToProxy, bool desToProxy, bool consoleOutput = false) : base(ipSrc, ipDes)
        {
            _clCenter = new UdpClient();
            _clCenter.Connect(_ipSrc);
            _centerCl = new UdpClient();
            _centerCl.Connect(_ipDes);
            if (srcToProxy) register.AddFlag(1);
            if (desToProxy) register.AddFlag(1 << 1);
            if (consoleOutput) register.AddFlag(1 << 2);
        }

        public Proxy(IPEndPoint ipSrc, IPEndPoint ipDes, Socket srcProx, Socket proxDes, bool srcToProxy, bool desToProxy, bool connect = true, bool consoleOutput = false) : base(ipSrc, ipDes)
        {
            _clCenter = new UdpClient();
            _clCenter.Client = srcProx;
            if(connect) _clCenter.Client.Connect(_ipSrc);
            _centerCl = new UdpClient();
            _centerCl.Client = proxDes;
            if (connect) _centerCl.Client.Connect(_ipDes);
            if (srcToProxy) register.AddFlag(1);
            if (desToProxy) register.AddFlag(1 << 1);
            if (consoleOutput) register.AddFlag(1 << 2);
        }

        public void GetPorts(out int clPortSide, out int desPortSide)
        {
            clPortSide = ((IPEndPoint) _clCenter.Client.LocalEndPoint).Port;
            desPortSide = ((IPEndPoint) _centerCl.Client.LocalEndPoint).Port;
        }

        public void CreateProxyAndBind(IPAddress ip)
        {
            IPEndPoint _ip = new IPEndPoint(ip, Ports.GetAvailablePort(49152));
            _clCenter = new UdpClient(_ip);
            _ip = new IPEndPoint(ip, Ports.GetAvailablePort(49152));
            _centerCl = new UdpClient(_ip);
        }

        public void ConnectToSource(IPEndPoint ipSrc)
        {
            _ipSrc = ipSrc;
            _clCenter.Connect(_ipSrc);
        }

        public void ConnectToDestination(IPEndPoint ipDes)
        {
            _ipDes = ipDes;
            _centerCl.Connect(_ipDes);
        }

        public void PrintProxy()
        {
            if (IsConnected())
            {
                Console.WriteLine("Proxy: Trace: " + 
                        _clCenter.Client.RemoteEndPoint + "->" + _clCenter.Client.LocalEndPoint + "->" + 
                        _centerCl.Client.LocalEndPoint + "->" + _centerCl.Client.RemoteEndPoint);
            }
        }

        public bool IsConnected()
        {
            return _centerCl.Client.Connected && _clCenter.Client.Connected;
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
            if (register.IsPresent(1)) Task.Run(() =>
            {
                while (!register.IsPresent(1 << 3))
                {
                    try
                    {
                        var r = _clCenter.Receive(ref _ipSrc);
                        if (r == null) continue;
                        if (r?.Length > 0)
                        {
                            if(register.IsPresent(1 << 2))
                            {
                                Console.Write("PX(C->P) ");
                                PrintNet.printRead(_clCenter.Client.LocalEndPoint, _ipSrc, r.Length);
                            }
                            int s = 0, total = r.Length;
                            do
                            {
                                r = ByteArray.SubArray(r, s);
                                s += _centerCl.Send(r, r.Length);
                                if(register.IsPresent(1 << 2))
                                {
                                    Console.Write("PX(P->D) ");
                                    PrintNet.printSend(_clCenter.Client.LocalEndPoint, _ipDes, s);
                                }
                            } while (s < total);
                        }
                    }
                    catch
                    {
                    }
                }
            });
            if (register.IsPresent(2)) Task.Run(() =>
            {
                while (!register.IsPresent(1 << 3))
                {
                    try
                    {
                        var r = _centerCl.Receive(ref _ipDes);
                        if (r == null) continue;
                        if (r?.Length > 0)
                        {
                            if (register.IsPresent(1 << 2))
                            {
                                Console.Write("PX(D->P) ");
                                PrintNet.printRead(_centerCl.Client.LocalEndPoint, _ipDes, r.Length);
                            }
                            int s = 0, total = r.Length;
                            do
                            {
                                r = ByteArray.SubArray(r, s);
                                s += _clCenter.Send(r, r.Length);
                                if (register.IsPresent(1 << 2))
                                {
                                    Console.Write("PX(P->C) ");
                                    PrintNet.printSend(_clCenter.Client.LocalEndPoint, _ipSrc, s);
                                }
                            } while (s != total);
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
