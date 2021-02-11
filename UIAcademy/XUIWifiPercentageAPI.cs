using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibUIAcademy
{
    public class XUIWifiPercentageAPI : Timer
    {
        // Fields
        private BackgroundWorker backgroundThread = new BackgroundWorker();
        private int Percentage = 100;
        private string ssid = "Not connected";

        // Methods
        public XUIWifiPercentageAPI()
        {
            this.Enabled = true;
            base.Interval = 0xbb8;
            this.backgroundThread.DoWork += new DoWorkEventHandler(this.backgroundThread_DoWork);
            this.backgroundThread.RunWorkerAsync();
        }

        private void backgroundThread_DoWork(object sender, DoWorkEventArgs e)
        {
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = "/C \"@echo off && for /f \"tokens=3 delims= \" %a in ('netsh wlan show interfaces ^| findstr /r \" ^....SSID\"') do echo %a && for /f \"tokens=3 delims= \" %a in ('netsh wlan show interfaces ^| findstr /r \" ^....Signal\"') do echo %a\""
            };
            process.StartInfo = info;
            process.Start();
            this.ssid = "Not connected";
            int num = 0;
            try
            {
                char[] separator = new char[] { ' ' };
                string[] strArray = process.StandardOutput.ReadToEnd().Split(separator);
                this.ssid = strArray[0];
                num = int.Parse(strArray[1].Remove(0, 2).Replace("%", ""));
                process.WaitForExit();
            }
            catch
            {
            }
            this.Percentage = num;
        }

        protected override void OnTick(EventArgs e)
        {
            base.OnTick(e);
            this.backgroundThread.RunWorkerAsync();
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Returns the wifi percentage")]
        public int Value =>
            this.Percentage;

        [Category("XanderUI"), Browsable(true), Description("Returns the SSID")]
        public string SSID =>
            this.ssid;
    }



}
