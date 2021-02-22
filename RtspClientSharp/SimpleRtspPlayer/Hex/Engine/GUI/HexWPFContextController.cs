using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using SimpleRtspPlayer.GUI.Models;
using SimpleRtspPlayer.GUI.ViewModels;
using SimpleRtspPlayer.GUI.Views.Main.WinForm;
using SimpleRtspPlayer.Hex.Connect;
using SimpleRtspPlayer.Hex.Engine.GUI.Exceptions;

namespace SimpleRtspPlayer.Hex.Engine.GUI
{
    public static class HexWpfContextController
    {
        private static bool _initLock = false;

        //Context Window
        private static IWpFwindow _contextWindow = null;
        private static IMainWindowModel _mainWindowModel = null;
        private static INotifyPropertyChanged _playerWindow = null;
        private static Grid _mainGrid = null;

        private static Form _mainForm = null;
        private static WindowsFormsHost _host = null;
        private static MainWindowViewModel _mainPlayerWindow = null;

        private static void _isInit()
        {
            if (!_initLock) throw new ControllerInitException("Controller is not initialized!", ErrorCode.NotInitialized);
        }

        public static void Init(IWpFwindow iwpFwindow)
        {
            if (_initLock) throw new ControllerInitException("Controller is already initialized!", ErrorCode.OnceInitialized);
            
            _contextWindow = iwpFwindow ?? throw new ArgumentNullException(nameof(iwpFwindow));
            _mainGrid = iwpFwindow.GetMainGrid() ?? throw new ArgumentNullException(nameof(iwpFwindow.GetMainGrid));
            _initLock = true;
        }

        public static void Release()
        {
            _initLock = false;
            _contextWindow = null;
            _mainGrid = null;
            _mainForm = null;
            _host = null;
        }

        private static void _setUpForm(Form form)
        {
            form.TopLevel = false;
            form.TopMost = true;
            form.Dock = DockStyle.Fill;
            form.FormBorderStyle = FormBorderStyle.None;
            form.AutoScaleMode = AutoScaleMode.None;
        }

        public static void StartWinForm()
        {
            _isInit();

            _host = new WindowsFormsHost();
            _mainForm ??= new FormMain();
            _setUpForm(_mainForm);
            _host.Child = _mainForm;

            _contextWindow.ShowWinForm(_host);
        }

        //false = connect
        public static void ExitWinForm(bool direct)
        {
            _isInit();

            _contextWindow.EndWinForm(direct);
        }

        public static void ShowPage(Page page)
        {
            _isInit();
            if (page == null) throw new ArgumentNullException(nameof(page));

            _contextWindow.ShowPage(page);
        }

        public static void ShowMainGrid()
        {
            _isInit();
            
            _contextWindow.ShowGrid(_mainGrid);
        }

        public static void SetUpPlayer(MainWindowViewModel mainWindowViewModel,IMainWindowModel iMainWindowModel)
        {
            _mainWindowModel = iMainWindowModel ?? throw new ArgumentNullException(nameof(iMainWindowModel));
            _mainPlayerWindow = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        }

        public static void StartPlayer()
        {
            if (!HexNetworkConnect.Check()) return;
            //_mainWindowModel.Start(HexNetworkConnect.GetConnectionParameters());
            _mainPlayerWindow.StartPlayer();
        }

        public static void StopPlayer()
        {
            _mainWindowModel.Stop();
        }
    }
}
