using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeetingClient.Forms;
using MeetingClient.Global;
using MeetingClient.Properties;

namespace MeetingClient
{
    static class Program
    {
        private static Mutex mutex = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main()
        {
            const string appName = "486578436c69656e74";

            mutex = new Mutex(true, appName, out var createdNew);

            if (!createdNew)
            {
                MessageBox.Show(Resources.AppAlreadyRun, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return (int) ExitCode.ALREADYRUN;
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormClient());

            return (int) ExitCode.SUCCESS;
        }
    }
}
