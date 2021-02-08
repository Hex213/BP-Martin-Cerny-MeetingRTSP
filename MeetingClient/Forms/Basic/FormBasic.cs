using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeetingClient.Forms.Create;

namespace MeetingClient.Forms.Basic
{
    public partial class FormBasic : Form
    {
        private Form _parent;

        public FormBasic()
        {
            InitializeComponent();
        }

        public FormBasic(Form parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            InitializeComponent();
            _parent = parent;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            FormCreateSettings formCreateSettings = new FormCreateSettings();
            _parent.Visible = false;
            formCreateSettings.ShowDialog();
        }
    }
}
