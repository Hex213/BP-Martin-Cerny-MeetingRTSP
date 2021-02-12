using System;
using MeetingClientWPF.GUI.WPF.Connect;
using SimpleRtspPlayer.GUI.Models;

namespace MeetingClientWPF.Engine
{
    class ViewModelLocator
    {
        private readonly Lazy<Connect_3> _mainWindowViewModelLazy =
            new Lazy<Connect_3>(CreateMainWindowViewModel);

        public Connect_3 MainWindowViewModel => _mainWindowViewModelLazy.Value;

        private static Connect_3 CreateMainWindowViewModel()
        {
            var model = new MainWindowModel();
            return new Connect_3(model);
        }
    }
}