using System;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using SimpleRtspPlayer.GUI.Views.Connect;
using SimpleRtspPlayer.GUI.Views.Main;
using SimpleRtspPlayer.Hex.Engine;
using SimpleRtspPlayer.Hex.GUI;
using SimpleRtspPlayer.Properties;

namespace SimpleRtspPlayer.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWpFwindow
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MultiplyByTen(int numberToMultiply);

        private Connect1 _pageConnect = null;
        private Page _pageCreate = null;

        private bool _tryRunHelper(string name, string sha)
        {
            return HSHA1.SHA1(Directory.GetCurrentDirectory() + @"\x86\" + name).ToLower() == sha;
        }

        private void _testCopy(string name, string sha, bool force)
        {
            if (File.Exists(Environment.SystemDirectory + @"\" + name) && !force) return;
            if (!_tryRunHelper(name, sha)) return;
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + @"\x86\" + name, Environment.SystemDirectory + @"\" + name, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void tryRun(bool force)
        {
            _testCopy("avcodec-58.dll", "61f42fd3a671d73c0025df2132a6c6f176c6a3b3", force);
            _testCopy("avdevice-58.dll", "af6e120bb55a19a7086ad72279633fc2f318567a", force);
            _testCopy("avfilter-7.dll", "1354ff296e8fe10f3540710170233448d014346e", force);
            _testCopy("avformat-58.dll", "09eb16ac1d75de55025997724d942a9dd4215aae", force);
            _testCopy("avutil-56.dll", "908784407c99fb78ae21a2fe9118e5ff95ca940a", force);
            _testCopy("libffmpeghelper.dll", "5ebac34d7e096ffe4edf3a9927e0c20d67fe247a", force);
            _testCopy("postproc-55.dll", "b30116a60dcb86e4f2e2735fb853a1554cc24d56", force);
            _testCopy("swresample-3.dll", "0e393f44b711fdb49a39b758bd5cdcaea3f4cdb5", force);
            _testCopy("swscale-5.dll", "5243d99be0458164fc1b46cca05b364b2d714dc0", force);
        }

        public MainWindow()
        {
            tryRun(false);
            InitializeComponent();
            HexWpfContextController.Init(this);
            HexWpfContextController.ShowPage(new MainPage());
            //HexWpfContextController.StartWinForm();
        }

        public void ShowWinForm(WindowsFormsHost wfh)
        {
            if (Settings.Default.LowRam)
            {
                _pageConnect = null;
                _pageCreate = null;
            }

            var page = new Empty();
            page.EmptyMainGrid.Children.Add(wfh);
            this.Content = page;
        }

        public void ShowPage(Page page)
        {
            this.Content = page ?? throw new ArgumentNullException(nameof(page));
        }

        public void ShowGrid(Grid grid)
        {
            this.Content = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public Grid GetMainGrid()
        {
            return this.MainGrid;
        }

        public void EndWinForm(bool type)
        {
            if(type)//create
            {
                //HexWPFContextController.ShowPage(_);
            }
            else//connect
            {
                HexWpfContextController.ShowPage(_pageConnect ??= new Connect1());
            }
        }
    }
}