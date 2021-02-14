using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace MeetingClientWPF.GUI.WPF.Connect
{
    /// <summary>
    /// Interaction logic for Connect_2.xaml
    /// </summary>
    public partial class Connect_2 : Page
    {
        public Connect_2()
        {
            InitializeComponent();
            //TODO: Check all ips in list
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            var ipAddress = HexConnectionSettings.Ip;
            long rectime = -1;

            if (IPAddress.TryParse(TextIP.Text, out ipAddress))
            {
                HexConnectionSettings.Ip = ipAddress;
            }
            else
            {
                //TODO: error - not connect
                return;
            }

            if (HexConnectionCheck.IsActive(TextIP.Text, 500, out rectime))
            {
                //TODO: detect if is slow or fast
                ContextController.ShowWpfPage(new Connect_3());
            }
            else
            {
                //TODO: error - not connect
            }
        }
    }
}
