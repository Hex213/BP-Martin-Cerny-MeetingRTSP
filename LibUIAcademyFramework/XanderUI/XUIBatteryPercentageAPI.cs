using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIBatteryPercentageAPI : Timer
    {
        // Fields
        private BackgroundWorker backgroundThread = new BackgroundWorker();
        private int Percentage;

        // Methods
        public XUIBatteryPercentageAPI()
        {
            this.Enabled = true;
            base.Interval = 0xbb8;
            this.backgroundThread.DoWork += new DoWorkEventHandler(this.backgroundThread_DoWork);
            this.backgroundThread.RunWorkerAsync();
        }

        private void backgroundThread_DoWork(object sender, DoWorkEventArgs e)
        {
            string str = SystemInformation.PowerStatus.BatteryLifePercent.ToString("#.#########", CultureInfo.InvariantCulture);
            this.Percentage = int.Parse(str.Substring(str.IndexOf(".") + 1));
            if (this.Percentage == 1)
            {
                this.Percentage = 100;
            }
        }

        protected override void OnTick(EventArgs e)
        {
            base.OnTick(e);
            this.backgroundThread.RunWorkerAsync();
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Returns the battery percentage")]
        public int Value =>
            this.Percentage;
    }



}
