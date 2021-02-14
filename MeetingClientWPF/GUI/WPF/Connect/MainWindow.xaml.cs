using System.Windows.Controls;
using MeetingClientWPF.GUI.Controllers;

namespace MeetingClientWPF.GUI.WPF.Connect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWPFWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void EndWinForm(bool type)
        {
            throw new System.NotImplementedException();
        }

        public void StartWinForm()
        {
            throw new System.NotImplementedException();
        }

        public void ShowPage(Page page)
        {
            throw new System.NotImplementedException();
        }

        public void HideForm()
        {
            this.Hide();
        }

        public void ShowForm()
        {
            this.Show();
        }
    }
}