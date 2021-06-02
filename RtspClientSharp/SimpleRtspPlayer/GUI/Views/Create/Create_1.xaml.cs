using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibRtspClientSharp.Hex;
using RtspClientSharp;
using SimpleRtspPlayer.Hex.Engine.GUI;

namespace SimpleRtspPlayer.GUI.Views.Create
{
    /// <summary>
    /// Interaction logic for Create_1.xaml
    /// </summary>
    public partial class Create_1 : Page
    {
        private bool _id = true, _pass = false, _tcp = false;
        private Create_2 _pageCreate = null;

        private void ChangeInfo(bool id, bool pass)
        {
            LabelIDStatus.Content = id ? "Set" : "Missing";
            LabelIDStatus.Foreground = id ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            LabelPassStatus.Content = pass ? "Set" : "Missing";
            LabelPassStatus.Foreground = pass ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }

        public Create_1()
        {
            InitializeComponent();
            ChangeInfo(_id, _pass);
        }

        private void saveAction()
        {
            if (id.Text.Length > 0)
            {
                _id = true;
            }

            _pass = pass.Password.Length > 0 && (encryption.IsChecked ?? false) || !(encryption.IsChecked ?? true);

            ChangeInfo(_id, _pass);
            run.IsEnabled = _id && _pass;
        }

        private void RunAction()
        {
            if (!(_id && _pass)) return;
            NetworkManager.InitConParams(new ConnectionParameters(new Uri("rtsp://127.0.0.1:1/"))
            {
                Enryption = encryption.IsChecked ?? false,
                UseServer = false,
                UseBase64 = false,
                RtpTransport = _tcp ? RtpTransportProtocol.TCP : RtpTransportProtocol.UDP
            });
            HexWpfContextController.ShowPage(_pageCreate ??= new Create_2(id.Text, pass.Password));
        }

        private void run_Click(object sender, RoutedEventArgs e)
        {
            RunAction();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            saveAction();
        }

        private void tcpButton_Checked(object sender, RoutedEventArgs e)
        {
            _tcp = true;
        }

        private void encryption_Checked(object sender, RoutedEventArgs e)
        {
            pass.IsEnabled = encryption.IsChecked ?? false;
        }

        private void encryption_Click(object sender, RoutedEventArgs e)
        {
            pass.IsEnabled = encryption.IsChecked ?? false;
        }

        private void udpButton_Checked(object sender, RoutedEventArgs e)
        {
            _tcp = false;
        }

        private void save_run_Click(object sender, RoutedEventArgs e)
        {
            saveAction();
            RunAction();
        }
    }
}
