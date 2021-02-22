using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using RtspClientSharp;
using SimpleRtspPlayer.GUI.Models;
using SimpleRtspPlayer.Hex.Connect;
using SimpleRtspPlayer.Hex.Engine.GUI;
using SimpleRtspPlayer.Hex.Globals;

namespace SimpleRtspPlayer.GUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _onceStart = false;
        private string _status = string.Empty;
        private readonly IMainWindowModel _mainWindowModel;

        public IVideoSource VideoSource => _mainWindowModel.VideoSource;
        
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(IMainWindowModel mainWindowModel)
        {
            _mainWindowModel = mainWindowModel ?? throw new ArgumentNullException(nameof(mainWindowModel));
            
            ClosingCommand = new RelayCommand<CancelEventArgs>(OnClosing);

            HexWpfContextController.SetUpPlayer(this, _mainWindowModel);
        }

        public void StartPlayer()
        {
            _mainWindowModel.Start(HexNetworkConnect.GetConnectionParameters(_onceStart));
            _mainWindowModel.StatusChanged += MainWindowModelOnStatusChanged;

            _onceStart = true;
        }

        public void StopPlayer()
        {
            _mainWindowModel.Stop();
            _mainWindowModel.StatusChanged -= MainWindowModelOnStatusChanged;
            
            Status = string.Empty;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindowModelOnStatusChanged(object sender, string s)
        {
            Application.Current.Dispatcher.Invoke(() => Status = s);
        }

        private void OnClosing(CancelEventArgs args)
        {
            StopPlayer();
        }
    }
}