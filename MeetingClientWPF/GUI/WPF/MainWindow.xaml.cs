using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using MeetingClientWPF.GUI.Controllers;
using MeetingClientWPF.GUI.WPF.Connect;
using XanderUI;

namespace MeetingClientWPF.GUI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWPFWindow
    {
        private WindowsFormsHost wfh;

        public MainWindow()
        {
            InitializeComponent();
            wfh = new WindowsFormsHost();
            ContextController.Init(grid1, this);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ContextController.Show(0);
        }

        public void EndWinForm(bool type)
        {
            
            if (type)//true - create
            {
                
            }
            else//false - connect
            {
                ContextController.ShowWpfPage(new Connect_1());
            }
        }

        public void StartWinForm()
        {
            ContextController.BackToStart();
        }

        public void ShowPage(Page page)
        {
            this.Content = page;
        }

        public void HideForm()
        {
            this.Hide();
        }

        public void ShowForm()
        {
            this.Show();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {

        }
    }
}
