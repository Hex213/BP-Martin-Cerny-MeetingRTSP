using System;
using System.Windows.Forms;
using SimpleRtspPlayer.Hex.Engine.GUI;
using SimpleRtspPlayer.Properties;

namespace SimpleRtspPlayer.GUI.Views.Main.WinForm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            butConnect.ButtonText = Resources.butConnect;
            butConnect.ButtonImage = Resources.ConnectImg;
            butCreate.ButtonText = Resources.butCreate;
            butCreate.ButtonImage = Resources.CreateImg;

        }

        private void butConnect_Click(object sender, EventArgs e)
        {
            HexWpfContextController.ExitWinForm(false);
        }

        private void butCreate_Click(object sender, EventArgs e)
        {
            HexWpfContextController.ExitWinForm(true);
        }
    }
}
