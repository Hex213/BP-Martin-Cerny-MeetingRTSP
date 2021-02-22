using System.Text.RegularExpressions;
using System.Windows.Controls;
using SimpleRtspPlayer.Hex.Connect;
using SimpleRtspPlayer.Hex.Engine.GUI;
using SimpleRtspPlayer.Hex.Globals;

namespace SimpleRtspPlayer.GUI.Views.Main
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private bool _oldInitLock = false;

        public MainPage()
        {
            _oldInitLock = true;
            InitializeComponent();
            _oldInitLock = false;
        }

        private void UserName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            var text = textBox.Text;

            text = Globals.rgxAZ.Replace(text, "");
            UserNameText.Text = text;
            UserNameText.CaretIndex = text.Length;
        }

        private void UserNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!_oldInitLock) ButtonConfirm.IsEnabled = UserNameText.Text.Length >= 3;
            //TODO: create hash
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HexNetworkConnect.AddName(UserNameText.Text))
            {
                HexWpfContextController.StartWinForm();
            }
            else
            {
                //TODO: bad name
            }
        }
    }
}
