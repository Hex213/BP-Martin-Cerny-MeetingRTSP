using System;
using MeetingClientWPF.Engine.Models;
using MeetingClientWPF.GUI.WPF.Connect;

namespace MeetingClientWPF.Engine
{
    class ViewModelLocator
    {
        private readonly Lazy<MainWindowViewModel> _mainWindowViewModelLazy =
            new Lazy<MainWindowViewModel>(CreateMainWindowViewModel);

        public MainWindowViewModel MainWindowViewModel => _mainWindowViewModelLazy.Value;

        private static MainWindowViewModel CreateMainWindowViewModel()
        {
            var model = new MainWindowModel();
            return new MainWindowViewModel(model);
        }
    }
}