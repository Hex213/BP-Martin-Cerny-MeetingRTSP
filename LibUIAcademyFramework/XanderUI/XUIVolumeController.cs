using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibUIAcademy.XanderUI
{
    public class XUIVolumeController : Component
    {
        // Fields
        private const byte VK_VOLUME_MUTE = 0xad;
        private const byte VK_VOLUME_DOWN = 0xae;
        private const byte VK_VOLUME_UP = 0xaf;
        private const uint KEYEVENTF_EXTENDEDKEY = 1;
        private const uint KEYEVENTF_KEYUP = 2;

        // Methods
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern byte MapVirtualKey(uint uCode, uint uMapType);
        public void Mute()
        {
            keybd_event(0xad, MapVirtualKey(0xad, 0), 1, 0);
            keybd_event(0xad, MapVirtualKey(0xad, 0), 3, 0);
        }

        public void SetVolume(int value)
        {
            int num = 0;
            while (num != 50)
            {
                keybd_event(0xae, MapVirtualKey(0xae, 0), 1, 0);
                keybd_event(0xae, MapVirtualKey(0xae, 0), 3, 0);
                num++;
            }
            if (value > 0)
            {
                for (num = 0; num != (value / 2); num++)
                {
                    keybd_event(0xaf, MapVirtualKey(0xaf, 0), 1, 0);
                    keybd_event(0xaf, MapVirtualKey(0xaf, 0), 3, 0);
                }
            }
        }

        public void VolumeDown()
        {
            keybd_event(0xae, MapVirtualKey(0xae, 0), 1, 0);
            keybd_event(0xae, MapVirtualKey(0xae, 0), 3, 0);
        }

        public void VolumeUp()
        {
            keybd_event(0xaf, MapVirtualKey(0xaf, 0), 1, 0);
            keybd_event(0xaf, MapVirtualKey(0xaf, 0), 3, 0);
        }
    }


}
