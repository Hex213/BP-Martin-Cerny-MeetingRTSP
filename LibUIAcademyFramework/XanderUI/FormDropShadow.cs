using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class FormDropShadow : Component
    {
        // Fields
        private Timer RefreshUI = new Timer();
        private int shadowAngle = 2;
        private Form effectedForm;
        private DropShadow ds = new DropShadow();

        // Methods
        public FormDropShadow()
        {
            this.RefreshUI.Interval = 50;
            this.RefreshUI.Tick += new EventHandler(this.RefreshUI_Tick);
            this.RefreshUI.Enabled = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.ds.Dispose();
        }

        private void Mainform_Resize(object sender, EventArgs e)
        {
            this.ds.Visible = this.effectedForm.WindowState == FormWindowState.Normal;
            if (this.ds.Visible)
            {
                Rectangle bounds = this.effectedForm.Bounds;
                this.ds.Bounds = bounds;
                this.ds.Location = new Point(this.effectedForm.Location.X + this.shadowAngle, this.effectedForm.Location.Y + this.shadowAngle);
            }
            this.effectedForm.BringToFront();
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
            Rectangle bounds = this.effectedForm.Bounds;
            this.ds.Bounds = bounds;
            this.ds.Location = new Point(this.effectedForm.Location.X + this.shadowAngle, this.effectedForm.Location.Y + this.shadowAngle);
            this.ds.Show();
            this.effectedForm.BringToFront();
        }

        private void RefreshUI_Tick(object sender, EventArgs e)
        {
            try
            {
                this.effectedForm.Shown += new EventHandler(this.Mainform_Shown);
                this.effectedForm.Resize += new EventHandler(this.Mainform_Resize);
                this.effectedForm.LocationChanged += new EventHandler(this.Mainform_Resize);
            }
            catch
            {
            }
        }

        // Properties
        public override ISite Site
        {
            get =>
                base.Site;
            set
            {
                base.Site = value;
                if (value != null)
                {
                    IDesignerHost service = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (service != null)
                    {
                        IComponent rootComponent = service.RootComponent;
                        if (rootComponent is ContainerControl)
                        {
                            this.effectedForm = rootComponent as Form;
                        }
                    }
                }
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Change the shadow angle, sorry this option is trial and error")]
        public int ShadowAngle
        {
            get =>
                this.shadowAngle;
            set =>
                this.shadowAngle = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The effected form(will remove ellipse from effected control)")]
        public Form EffectedForm
        {
            get =>
                this.effectedForm;
            set =>
                this.effectedForm = value;
        }

        // Nested Types
        public class DropShadow : Form
        {
            // Fields
            private const int WS_EX_TRANSPARENT = 0x20;
            private const int WS_EX_NOACTIVATE = 0x800_0000;

            // Methods
            public DropShadow()
            {
                this.BackColor = Color.Black;
                base.Opacity = 0.3;
                base.ShowInTaskbar = false;
                base.FormBorderStyle = FormBorderStyle.None;
                base.StartPosition = FormStartPosition.Manual;
            }

            // Properties
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams createParams = base.CreateParams;
                    createParams.ExStyle = (createParams.ExStyle | 0x20) | 0x800_0000;
                    return createParams;
                }
            }
        }
    }
}
