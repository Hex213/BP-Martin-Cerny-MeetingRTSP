using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIObjectEllipse : Component
    {
        // Fields
        private Timer RefreshUI = new Timer();
        private Control DefaultControl;
        private FormBorderStyle DefaultStyle;
        private Region DefaultRegion;
        private int cornerRadius = 10;
        private Control effectedControl;

        // Methods
        public XUIObjectEllipse()
        {
            this.RefreshUI.Interval = 50;
            this.RefreshUI.Tick += new EventHandler(this.RefreshUI_Tick);
            this.RefreshUI.Enabled = true;
        }

        private void Container_SizeChanged(object sender, EventArgs e)
        {
            this.effectedControl.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.effectedControl.Width, this.effectedControl.Height, this.cornerRadius, this.cornerRadius));
        }

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.effectedControl != null)
            {
                try
                {
                    ((Form)this.effectedControl).FormBorderStyle = this.DefaultStyle;
                }
                catch
                {
                }
                this.effectedControl.Region = this.DefaultRegion;
            }
        }

        private void RefreshUI_Tick(object sender, EventArgs e)
        {
            this.UpdateControl();
            this.RefreshUI.Dispose();
        }

        private void SetCustomRegion()
        {
            if (this.effectedControl != null)
            {
                try
                {
                    ((Form)this.effectedControl).FormBorderStyle = FormBorderStyle.None;
                }
                catch
                {
                }
                this.effectedControl.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.effectedControl.Width, this.effectedControl.Height, this.cornerRadius, this.cornerRadius));
                this.effectedControl.SizeChanged += new EventHandler(this.Container_SizeChanged);
            }
        }

        private void UpdateControl()
        {
            if (this.DefaultControl != null)
            {
                try
                {
                    ((Form)this.DefaultControl).FormBorderStyle = this.DefaultStyle;
                }
                catch
                {
                }
                this.DefaultControl.Region = this.DefaultRegion;
            }
            if (this.effectedControl != null)
            {
                try
                {
                    this.DefaultControl = (Form)this.effectedControl;
                }
                catch
                {
                    this.DefaultControl = this.effectedControl;
                }
                this.DefaultRegion = this.effectedControl.Region;
                try
                {
                    this.DefaultStyle = ((Form)this.effectedControl).FormBorderStyle;
                }
                catch
                {
                }
                this.SetCustomRegion();
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
                            this.effectedControl = rootComponent as ContainerControl;
                            this.DefaultControl = rootComponent as ContainerControl;
                            this.DefaultRegion = this.DefaultControl.Region;
                            try
                            {
                                this.DefaultStyle = ((Form)this.DefaultControl).FormBorderStyle;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The corner radius")]
        public int CornerRadius
        {
            get =>
                this.cornerRadius;
            set
            {
                this.cornerRadius = value;
                this.SetCustomRegion();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The effected control")]
        public Control EffectedControl
        {
            get =>
                this.effectedControl;
            set
            {
                this.effectedControl = value;
                this.UpdateControl();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The effected form(will remove ellipse from effected control)")]
        public Form EffectedForm
        {
            get
            {
                try
                {
                    return (this.effectedControl as Form);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                this.effectedControl = value;
                this.UpdateControl();
            }
        }
    }



}
