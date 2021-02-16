using System;
using System.Net.Http.Headers;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using SimpleRtspPlayer.GUI.Views.Connect;
using SimpleRtspPlayer.GUI.Views.Create;
using SimpleRtspPlayer.GUI.Views.Main;
using SimpleRtspPlayer.Hex.GUI;
using SimpleRtspPlayer.Properties;

namespace SimpleRtspPlayer.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWpFwindow
    {
        private Connect1 _pageConnect = null;
        private Create_1 _pageCreate = null;

        public MainWindow()
        {
            InitializeComponent();
            HexWpfContextController.Init(this);
            HexWpfContextController.ShowPage(new MainPage());
        }

        private void _clear()
        {
            if (!Settings.Default.LowRam) return;
            _pageConnect = null;
            _pageCreate = null;
        }

        public void ShowWinForm(WindowsFormsHost wfh)
        {
            _clear();

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
            if (type)//create
            {
                HexWpfContextController.ShowPage(_pageCreate ??= new Create_1());
            }
            else//connect
            {
                HexWpfContextController.ShowPage(_pageConnect ??= new Connect1());
            }
            _clear();
        }
    }
}