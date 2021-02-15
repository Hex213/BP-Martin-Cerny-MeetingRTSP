using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimpleRtspPlayer.Hex.Connect;
using SimpleRtspPlayer.Hex.Globals;
using SimpleRtspPlayer.Hex.GUI;

namespace SimpleRtspPlayer.GUI.Views.Connect
{
    /// <summary>
    /// Interaction logic for Connect_2.xaml
    /// </summary>
    public partial class Connect2 : Page
    {
        public Connect2()
        {
            InitializeComponent();
            //TODO: Check all ips in list
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

            //TODO: Show wait -> check ping
            HexNetworkConnect.AddCredential(new SecureString());
            HexWpfContextController.ShowMainGrid();
            HexWpfContextController.StartPlayer();
            
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
    }
}
