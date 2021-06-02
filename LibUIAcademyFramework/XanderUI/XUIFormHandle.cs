using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIFormHandle : Component
    {
        // Fields
        private Control handleControl;
        private bool dockAtTop = true;
        public const int WM_NCLBUTTONDOWN = 0xa1;
        public const int HT_CAPTION = 2;

        // Methods
        private void DragForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.HandleControl.FindForm().Handle, 0xa1, 2, 0);
                if (this.dockAtTop && (this.handleControl.FindForm().FormBorderStyle == FormBorderStyle.None))
                {
                    if ((this.HandleControl.FindForm().WindowState != FormWindowState.Maximized) && (Cursor.Position.Y <= 3))
                    {
                        this.HandleControl.FindForm().WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        this.HandleControl.FindForm().WindowState = FormWindowState.Normal;
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The handleControl")]
        public Control HandleControl
        {
            get =>
                this.handleControl;
            set
            {
                this.handleControl = value;
                this.handleControl.MouseDown += new MouseEventHandler(this.DragForm_MouseDown);
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Maximize when dragged to top")]
        public bool DockAtTop
        {
            get =>
                this.dockAtTop;
            set =>
                this.dockAtTop = value;
        }
    }


}
