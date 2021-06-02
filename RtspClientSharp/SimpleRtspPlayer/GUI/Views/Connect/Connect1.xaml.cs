using System;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibRtspClientSharp.Hex;
using RtspClientSharp;
using SimpleRtspPlayer.Hex.Connect;
using SimpleRtspPlayer.Hex.Engine.GUI;
using SimpleRtspPlayer.Hex.Globals;
using SimpleRtspPlayer.Properties;

namespace SimpleRtspPlayer.GUI.Views.Connect
{
    /// <summary>
    /// Interaction logic for Connect_1.xaml
    /// </summary>
    public partial class Connect1 : Page
    {
        private Connect2 _connectNext = null;
        private bool _oldInitLock = false;

        public Connect1()
        {
            _oldInitLock = true;
            InitializeComponent();
            _oldInitLock = false;
            if (Settings.Default.Remember)
            {
                TextID.Text = Settings.Default.IDconnect;
            }
        }

        private void _activate(bool activate)
        {
            ButtonConfirm.IsEnabled = activate;
            if (activate)
            {
                //TODO: dokoncit vypinanie
                //NextImage.ImageSource = ;
                //BackImage.ImageSource = ;
            }
            else
            {
                //NextImage.ImageSource = ;
                //BackImage.ImageSource = ;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_oldInitLock)
            {
                _activate(TextID.Text.Length >= NetworkGlobal.MinIdLength);
            }
        }

        private void _next()
        {
            //TODO: add worker and effect - increasing and visibility
            /*
             * 650-575=75//11,5
               384-341=43//11,2
               
               550x284
             */

            if (!HexNetworkConnect.AddId(TextID.Text))
            {
                throw new ArgumentException("Not valid ID");
            }
            if (Settings.Default.Remember)
            {
                Settings.Default.IDconnect = TextID.Text;
                Settings.Default.Save();
            }

            HexWpfContextController.ShowPage(_connectNext ??= new Connect2());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NetworkManager.InitConParams(new ConnectionParameters(new Uri("rtsp://1.1.1.1:1/")) {
                Suffix = TextID.Text
            });

            _next();
        }
        
        private void BackImageRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Settings.Default.LowRam)
                {
                    this._connectNext = null;
                }

                HexWpfContextController.StartWinForm();
            }
        }

        private void TextID_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            var text = textBox.Text;

            text = Globals.rgxAZ.Replace(text, "");
            TextID.Text = text;
            ButtonConfirm.IsEnabled = text.Length >= 3;
            TextID.CaretIndex = text.Length;
        }
    }
}