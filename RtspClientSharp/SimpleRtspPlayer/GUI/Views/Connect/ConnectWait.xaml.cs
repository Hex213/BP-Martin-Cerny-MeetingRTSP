using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LibRtspClientSharp.Hex;
using RtspClientSharp;
using SimpleRtspPlayer.Hex.Engine.GUI;

namespace SimpleRtspPlayer.GUI.Views.Connect
{
    /// <summary>
    /// Interaction logic for Connect_Wait.xaml
    /// </summary>
    public partial class ConnectWait : Page
    {
        private bool tcp = false, encr = true;
        public ConnectWait()
        {
            InitializeComponent();
            LabelID.Content = "Vaše ID:" + CipherManager.GetID();
            EncrYesButton.IsChecked = true;
            EncrNoButton.IsChecked = false;
            udpButton.IsChecked = true;
            tcpButton.IsChecked = false;
        }

        private void udpButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            tcp = !udpButton.IsChecked ?? true;
        }

        private void tcpButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            tcp = tcpButton.IsChecked ?? false;
        }

        private void EncrYesButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            encr = EncrYesButton.IsChecked ?? false;
        }

        private void EncrNoButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            encr = !EncrNoButton.IsChecked ?? true;
        }

        private void status_change(string status)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                LabelStatus.Content = "Status: " + status;
            }), DispatcherPriority.Render);
        }

        private void Connect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Connect.IsEnabled = false;
            status_change("Connecting...");

            var suffix = NetworkManager.ConnectionParameters.Suffix;
            NetworkManager.ConnectionParameters.Enryption = encr;
            NetworkManager.ConnectionParameters.RtpTransport =
                tcp ? RtpTransportProtocol.TCP : RtpTransportProtocol.UDP;

            var ipSes = NetworkManager.ConnMet(suffix);
            if (ipSes == null)
            {
                Connect.IsEnabled = true;
                status_change("Somethings wrong!\n" + NetworkManager.errMsg);
                return;
            }
            var serverUri = new Uri("rtsp://" + ipSes.Address + ":" + ipSes.Port + "/" + suffix);
            
            NetworkManager.updateUri(serverUri);

            status_change("First phase .. connected!");

            HexWpfContextController.ShowMainGrid();
            HexWpfContextController.StartPlayer();
        }
    }
}