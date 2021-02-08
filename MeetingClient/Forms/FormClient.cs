using System.Windows.Forms;
using MeetingClient.Forms.Basic;

namespace MeetingClient.Forms
{
    public partial class FormClient : Form
    {
        private FormBasic _formBasic = null;

        public FormClient()
        {
            InitializeComponent();
            _formBasic = new FormBasic(this) {TopLevel = false, TopMost = true, Dock = DockStyle.Fill};
        }

        private void FormClient_Load(object sender, System.EventArgs e)
        {
            //TODO: LOADING - form, load basic components, test network connection

            //TODO: if load correct then:
            this.mainViewPanel.Controls.Add(_formBasic);
        }

        private void FormClient_Shown(object sender, System.EventArgs e)
        {
            _formBasic.Show();
        }
    }
}
