using System.Windows.Forms;

namespace MeetingClient.Forms.Create
{
    public partial class FormCreateSettings : Form
    {
        public FormCreateSettings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            FormCreateChangeServer formCreateChangeServer = new FormCreateChangeServer();
            this.Hide();
            formCreateChangeServer.ShowDialog();
        }
    }
}
