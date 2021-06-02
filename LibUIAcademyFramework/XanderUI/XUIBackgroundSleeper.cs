using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIBackgroundSleeper : Component
    {
        // Methods
        public void Sleep(int milliseconds)
        {
            DateTime time = DateTime.Now.AddMilliseconds((double)milliseconds);
            while (DateTime.Now < time)
            {
                Application.DoEvents();
            }
        }
    }


}
