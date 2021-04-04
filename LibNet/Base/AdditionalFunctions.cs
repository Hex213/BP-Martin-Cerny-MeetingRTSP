using System;
using System.Collections.Generic;
using System.Text;
using LibNet.Utils;

namespace LibNet.Base
{
    public abstract class AdditionalFunctions
    {
        public abstract bool RecvCondition(object args);
        public abstract void RecvFunc(object args);
        public abstract bool SendCondition(object args);
        public abstract void SendFunc(object args);
    }
}
