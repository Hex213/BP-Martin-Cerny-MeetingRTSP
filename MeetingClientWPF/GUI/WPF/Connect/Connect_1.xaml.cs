using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
using MeetingClientWPF.GUI.Controllers;
using MeetingClientWPF.Hex;
using MeetingClientWPF.Properties;

namespace MeetingClientWPF.GUI.WPF.Connect
{
    /// <summary>
    /// Interaction logic for Connect_1.xaml
    /// </summary>
    public partial class Connect_1 : Page
    {
        private bool _oldInitLock = false;

        public Connect_1()
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
                _activate(TextID.Text.Length >= 3);
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
            HexConnectionSettings.Id = TextID.Text;
            if (Settings.Default.Remember)
            {
                Settings.Default.IDconnect = TextID.Text;
                Settings.Default.Save();
            }

            ContextController.ShowPage(new Connect_2());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _next();
        }
    }
}