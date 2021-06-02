using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using LibRtspClientSharp.Hex;

namespace SimpleRtspPlayer.GUI.Views.Create
{
    /// <summary>
    /// Interaction logic for Create_2.xaml
    /// </summary>
    public partial class Create_2 : Page
    {
        private void updateScreen()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var id = NetworkManager.ConnArrived();
                    string Request = "Nové pripojenie od id: " + id;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        LabelRequest.Content = Request;
                        ConfirmButton.IsEnabled = true;
                        DenyButton.IsEnabled = true;
                    }), DispatcherPriority.ContextIdle);

                }
            });
        }

        public Create_2(string id, string key) : this()
        {
            //todo: link to confirm
            Task.Run(() => { NetworkManager.HostMet(id, key); });
            Task.Run(() => updateScreen());
        }

        public Create_2()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkManager.SetReply(true);
            ConfirmButton.IsEnabled = false;
            DenyButton.IsEnabled = false;
            LabelRequest.Content = "Nothing";
        }

        private void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkManager.SetReply(false);
            ConfirmButton.IsEnabled = false;
            DenyButton.IsEnabled = false;
            LabelRequest.Content = "Nothing";
        }
    }
}
