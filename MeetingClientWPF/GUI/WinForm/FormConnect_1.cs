using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeetingClientWPF.GUI.Controllers;

namespace MeetingClientWPF.GUI.WinForm
{
    public partial class FormConnect_1 : Form
    {
        public FormConnect_1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ContextController.Back();
        }

        private void xuiButton1_Click(object sender, EventArgs e)
        {
            ContextController.Back();
        }
    }
}
