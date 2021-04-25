using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LibHexUtils.Register;

namespace LibNet.Base
{
    public abstract class Proxy
    {
        protected IPEndPoint _ipSrc;
        protected IPEndPoint _ipDes;

        // Register info
        // [...Free][Running][ExitProxy][ConsOutput][desToProxy][srcToProxy]LSB
        protected Register register;

        protected Proxy()
        {
        }

        protected Proxy(IPEndPoint ipSrc, IPEndPoint ipDes)
        {
            register = new Register();
            _ipSrc = ipSrc;
            _ipDes = ipDes;
        }

        public abstract void StopProxy();
        public abstract void StartProxy();
    }
}
