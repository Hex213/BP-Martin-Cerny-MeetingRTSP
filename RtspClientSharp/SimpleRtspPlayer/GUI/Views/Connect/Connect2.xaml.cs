using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibRtspClientSharp.Hex;
using RtspClientSharp;
using SimpleRtspPlayer.Hex.Connect;
using SimpleRtspPlayer.Hex.Engine.GUI;
using SimpleRtspPlayer.Hex.Globals;

namespace SimpleRtspPlayer.GUI.Views.Connect
{
    public class ItemIP
    {
        public ItemIP(string ip, string official, string conn, string ping)
        {
            IPAddress = ip;
            OfficialServer = official;
            ConnStatus = conn;
            Ping = ping;
        }

        public string IPAddress { get; set; }

        public string OfficialServer { get; set; }

        public string ConnStatus { get; set; }

        public string Ping { get; set; }
    }

    /// <summary>
    /// Interaction logic for Connect_2.xaml
    /// </summary>
    public partial class Connect2 : Page
    {
        private ConnectWait _connectWait = null;
        public Connect2()
        {
            InitializeComponent();
            var i = new ItemIP("127.0.0.1:40000", "No", "Unknown", "Unknown");
            iptable.Items.Add(i);
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!HexNetworkConnect.AddIp(TextIP.Text))
            {
                //TODO: show incorrect IPuri
                return;
            }

            if (!HexNetworkConnect.AddPort(TextPort.Text))
            {
                //TODO: show incorrect Port
                return;
            }

            string suf = NetworkManager.ConnectionParameters.Suffix;
            NetworkManager.InitConParams(new ConnectionParameters(new Uri("rtsp://" + TextIP.Text + ":" + TextPort.Text + "/"))
            {
                Suffix = suf
            });
            HexNetworkConnect.AddCredential(new SecureString());
            NetworkManager.ConnectBase(HexNetworkConnect.IpAddr, HexNetworkConnect.Port, 2, 5);
            HexWpfContextController.ShowPage(_connectWait ??= new ConnectWait());
        }

        private void TextPort_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            var text = textBox.Text;

            text = Globals.rgx09.Replace(text, "");
            TextPort.Text = text;
            TextPort.CaretIndex = text.Length;
        }

        private void TextIP_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            var text = textBox.Text;

            text = Globals.rgx09b.Replace(text, "");
            TextIP.Text = text;
            TextIP.CaretIndex = text.Length;
        }

        private void iptable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                var iprow = (ItemIP)item.Content;
                var i = iprow.IPAddress.IndexOf(':');
                var tmp = iprow.IPAddress.Substring(0, i);

                if (i == -1) return;

                if (IPAddress.TryParse(tmp, out var ip))
                {
                    tmp = iprow.IPAddress.Substring(i+1);
                    if (Int32.TryParse(tmp, out var port))
                    {
                        TextIP.Text = ip.ToString();
                        TextPort.Text = tmp;
                    }
                }
            }
        }
    }
}
