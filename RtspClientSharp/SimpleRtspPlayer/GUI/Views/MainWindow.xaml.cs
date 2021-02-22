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
using SimpleRtspPlayer.Hex.Engine.GUI;
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

        

        

        

        public MainWindow()
        {
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