using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSNamedPipe;

namespace MeetingClient.Forms.Create
{
    public partial class FormCreateChangeServer : Form
    {
        private NamedPipeServer Pin;
        private NamedPipeServer Pout;

        public FormCreateChangeServer()
        {
            InitializeComponent();
        }

        private void FormCreateChangeServer_Load(object sender, EventArgs e)
        {
            Pin = new NamedPipeServer(@"\\.\pipe\serverIN", 0);
            Pout = new NamedPipeServer(@"\\.\pipe\serverOUT", 1);

            do
            {
                Pin.Start();
            } while (!Pin.IsStarted);

            do
            {
                Pout.Start();
            } while (!Pout.IsStarted);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Pin.InputFromPipe.Count.ToString();
            label2.Text = Pout.InputFromPipe.Count.ToString();
        }
    }
}
