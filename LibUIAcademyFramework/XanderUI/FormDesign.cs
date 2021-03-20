using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibUIAcademy.Designers;

namespace LibUIAcademyFramework.XanderUI
{
    [Designer(typeof(FormDesigner))]
    public class XUIFormDesign : Panel
    {
        // Fields
        private bool SetBG;
        private Panel WorkPanel = new Panel();
        private PictureBox minimize = new PictureBox();
        private PictureBox exit = new PictureBox();
        private PictureBox maximize = new PictureBox();
        private FormBorderStyle DefaultStyle;
        private Form DefaultForm;
        private string titleText = "Form Name";
        private bool showMaximize = true;
        private bool exitApplication = true;
        private bool showMinimize = true;
        private Color materialForeColor = Color.White;
        private Color materialBackColor = Color.DodgerBlue;
        private Style formStyle = Style.MacOS;

        // Methods
        public XUIFormDesign()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.DefaultForm.FormBorderStyle = this.DefaultStyle;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            if (this.exitApplication)
            {
                Application.Exit();
            }
            else
            {
                base.FindForm().Close();
            }
        }

        private void Exit_MouseEnter(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(0x11, 0x11);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0xeb, 0x5f, 80)), new Rectangle(1, 1, 15, 15));
            graphics1.DrawLine(new Pen(Color.Black, 1f), 6, 6, 11, 11);
            graphics1.DrawLine(new Pen(Color.Black, 1f), 6, 11, 11, 6);
            this.exit.Image = image;
        }

        private void Exit_MouseLeave(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(0x11, 0x11);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0xeb, 0x5f, 80)), new Rectangle(1, 1, 15, 15));
            this.exit.Image = image;
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            this.RefreshUI();
            base.ResumeLayout(true);
        }

        private void Maximize_Click(object sender, EventArgs e)
        {
            if (base.FindForm().WindowState == FormWindowState.Normal)
            {
                base.FindForm().WindowState = FormWindowState.Maximized;
            }
            else
            {
                base.FindForm().WindowState = FormWindowState.Normal;
            }
        }

        private void Maximize_MouseEnter(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(0x11, 0x11);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0xf5, 190, 50)), new Rectangle(1, 1, 15, 15));
            graphics1.DrawRectangle(new Pen(Color.Black, 1f), new Rectangle(6, 6, 6, 6));
            this.maximize.Image = image;
        }

        private void Maximize_MouseLeave(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(0x11, 0x11);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0xf5, 190, 50)), new Rectangle(1, 1, 15, 15));
            this.maximize.Image = image;
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            base.FindForm().WindowState = FormWindowState.Minimized;
        }

        private void Minimize_MouseEnter(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(0x11, 0x11);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0, 0xcd, 90)), new Rectangle(1, 1, 15, 15));
            graphics1.DrawLine(new Pen(Color.Black, 1f), 6, 9, 11, 9);
            this.minimize.Image = image;
        }

        private void Minimize_MouseLeave(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(0x11, 0x11);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0, 0xcd, 90)), new Rectangle(1, 1, 15, 15));
            this.minimize.Image = image;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            this.SetBG = true;
            this.WorkingArea.BackColor = this.BackColor;
        }

        private void RefreshUI()
        {
            base.SuspendLayout();
            base.Controls.Clear();
            this.Dock = DockStyle.Fill;
            try
            {
                base.FindForm().FormBorderStyle = FormBorderStyle.None;
            }
            catch
            {
            }
            Label label = new Label();
            XUIFormHandle handle = new XUIFormHandle();
            XUIFormHandle handle1 = new XUIFormHandle();
            if (this.formStyle == Style.Material)
            {
                Panel panel = new Panel();
                Panel panel2 = new Panel();
                XUIButton button = new XUIButton();
                XUIButton button2 = new XUIButton();
                XUIButton button3 = new XUIButton();
                XUIFormHandle handle2 = new XUIFormHandle();
                panel2.BackColor = this.MaterialBackColor;
                panel2.Location = new Point(0, 0);
                panel2.Size = new Size(base.Width, 50);
                panel2.Dock = DockStyle.Top;
                panel.BackColor = this.MaterialBackColor;
                panel.Location = new Point(0, 0);
                panel.Size = new Size(base.Width, 0x18);
                panel.Dock = DockStyle.Top;
                if (this.showMinimize)
                {
                    button3.Size = new Size(0x18, 0x18);
                    button3.Dock = DockStyle.Right;
                    button3.ButtonText = "_";
                    button3.TextColor = this.MaterialForeColor;
                    button3.BackgroundColor = panel.BackColor;
                    button3.ClickBackColor = panel.BackColor;
                    button3.ButtonStyle = XUIButton.Style.Material;
                    button3.ButtonImage = null;
                    button3.Click += new EventHandler(this.Minimize_Click);
                    panel.Controls.Add(button3);
                }
                if (this.showMaximize)
                {
                    button2.Size = new Size(0x18, 0x18);
                    button2.Dock = DockStyle.Right;
                    button2.ButtonText = "[  ]";
                    button2.TextColor = this.MaterialForeColor;
                    button2.BackgroundColor = panel.BackColor;
                    button2.ClickBackColor = panel.BackColor;
                    button2.ButtonStyle = XUIButton.Style.Material;
                    button2.ButtonImage = null;
                    button2.Click += new EventHandler(this.Maximize_Click);
                    panel.Controls.Add(button2);
                }
                button.Size = new Size(0x18, 0x18);
                button.Dock = DockStyle.Right;
                button.ButtonText = "X";
                button.TextColor = this.MaterialForeColor;
                button.BackgroundColor = panel.BackColor;
                button.HoverBackgroundColor = Color.Red;
                button.ClickBackColor = Color.Red;
                button.ButtonText = "X";
                button.ButtonStyle = XUIButton.Style.Material;
                button.ButtonImage = null;
                button.Click += new EventHandler(this.Exit_Click);
                panel.Controls.Add(button);
                label.ForeColor = this.MaterialForeColor;
                label.Font = new Font("Calibri", 12f);
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.AutoSize = false;
                label.Dock = DockStyle.Left;
                label.Text = this.titleText;
                panel.Controls.Add(label);
                this.WorkPanel.Location = new Point(0, 50);
                this.BackColor = Color.White;
                this.WorkPanel.Dock = DockStyle.Fill;
                handle.HandleControl = label;
                handle.HandleControl = panel;
                handle.HandleControl = panel2;
                base.Controls.Add(this.WorkPanel);
                base.Controls.Add(panel2);
                base.Controls.Add(panel);
            }
            if (this.formStyle == Style.MacOS)
            {
                Panel panel3 = new Panel
                {
                    BackColor = Color.FromArgb(0xad, 0xad, 0xad),
                    Size = new Size(base.Width, 1),
                    Dock = DockStyle.Top
                };
                XUIGradientPanel panel4 = new XUIGradientPanel
                {
                    Style = XUIGradientPanel.GradientStyle.Horizontal,
                    TopLeft = Color.FromArgb(230, 230, 230),
                    TopRight = Color.FromArgb(210, 210, 210),
                    Size = new Size(base.Width, 0x26),
                    Dock = DockStyle.Top
                };
                label.ForeColor = Color.FromArgb(40, 40, 40);
                label.BackColor = Color.Transparent;
                label.Parent = panel4;
                label.Font = new Font("Microsoft Sans Serif", 10f);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.AutoSize = false;
                label.Dock = DockStyle.Fill;
                label.Text = this.titleText;
                panel4.Controls.Add(label);
                int x = 40;
                if (!this.showMaximize)
                {
                    x = 20;
                }
                if (this.showMinimize)
                {
                    this.minimize.BackColor = Color.Transparent;
                    this.minimize.Parent = panel4;
                    this.minimize.Size = new Size(20, 40);
                    this.minimize.Location = new Point(x, 0);
                    Bitmap bitmap2 = new Bitmap(0x11, 0x11);
                    Graphics graphics1 = Graphics.FromImage(bitmap2);
                    graphics1.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics1.FillEllipse(new SolidBrush(Color.FromArgb(0, 0xcd, 90)), new Rectangle(1, 1, 15, 15));
                    this.minimize.Image = bitmap2;
                    this.minimize.Click += new EventHandler(this.Minimize_Click);
                    this.minimize.MouseEnter += new EventHandler(this.Minimize_MouseEnter);
                    this.minimize.MouseLeave += new EventHandler(this.Minimize_MouseLeave);
                    this.minimize.SizeMode = PictureBoxSizeMode.CenterImage;
                    panel4.Controls.Add(this.minimize);
                    this.minimize.BringToFront();
                }
                if (this.showMaximize)
                {
                    this.maximize.BackColor = Color.Transparent;
                    this.maximize.Parent = panel4;
                    this.maximize.Size = new Size(20, 40);
                    this.maximize.Location = new Point(20, 0);
                    Bitmap bitmap3 = new Bitmap(0x11, 0x11);
                    Graphics graphics2 = Graphics.FromImage(bitmap3);
                    graphics2.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics2.FillEllipse(new SolidBrush(Color.FromArgb(0xf5, 190, 50)), new Rectangle(1, 1, 15, 15));
                    this.maximize.Image = bitmap3;
                    this.maximize.Click += new EventHandler(this.Maximize_Click);
                    this.maximize.MouseEnter += new EventHandler(this.Maximize_MouseEnter);
                    this.maximize.MouseLeave += new EventHandler(this.Maximize_MouseLeave);
                    this.maximize.SizeMode = PictureBoxSizeMode.CenterImage;
                    panel4.Controls.Add(this.maximize);
                    this.maximize.BringToFront();
                }
                this.exit.BackColor = Color.Transparent;
                this.exit.Parent = panel4;
                this.exit.Size = new Size(20, 40);
                this.exit.Location = new Point(0, 0);
                Bitmap image = new Bitmap(0x11, 0x11);
                Graphics graphics3 = Graphics.FromImage(image);
                graphics3.SmoothingMode = SmoothingMode.AntiAlias;
                graphics3.FillEllipse(new SolidBrush(Color.FromArgb(0xeb, 0x5f, 80)), new Rectangle(1, 1, 15, 15));
                this.exit.Image = image;
                this.exit.Click += new EventHandler(this.Exit_Click);
                this.exit.MouseEnter += new EventHandler(this.Exit_MouseEnter);
                this.exit.MouseLeave += new EventHandler(this.Exit_MouseLeave);
                this.exit.SizeMode = PictureBoxSizeMode.CenterImage;
                panel4.Controls.Add(this.exit);
                this.exit.BringToFront();
                this.WorkPanel.Location = new Point(0, 50);
                if (!this.SetBG)
                {
                    this.BackColor = Color.FromArgb(0xec, 0xec, 0xec);
                    this.SetBG = true;
                }
                this.WorkPanel.BackColor = this.BackColor;
                this.WorkPanel.Dock = DockStyle.Fill;
                handle.HandleControl = label;
                handle.HandleControl = panel4;
                base.Controls.Add(this.WorkPanel);
                base.Controls.Add(panel3);
                base.Controls.Add(panel4);
            }
            if (this.formStyle == Style.Ubuntu)
            {
                XUIGradientPanel panel5 = new XUIGradientPanel
                {
                    Style = XUIGradientPanel.GradientStyle.Horizontal,
                    TopLeft = Color.FromArgb(90, 0x55, 80),
                    TopRight = Color.FromArgb(0x41, 0x41, 60),
                    Size = new Size(base.Width, 30),
                    Dock = DockStyle.Top
                };
                label.ForeColor = Color.FromArgb(220, 220, 210);
                label.BackColor = Color.Transparent;
                label.Parent = panel5;
                label.Size = new Size(base.Width - 50, 30);
                label.Location = new Point(0x4b, 0);
                label.Font = new Font("Arial", 10f, FontStyle.Bold);
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.AutoSize = false;
                label.Text = this.titleText;
                panel5.Controls.Add(label);
                int x = 50;
                if (!this.showMinimize)
                {
                    x = 0x19;
                }
                if (this.showMaximize)
                {
                    this.maximize.BackColor = Color.Transparent;
                    this.maximize.Parent = panel5;
                    this.maximize.Size = new Size(0x19, 30);
                    this.maximize.Location = new Point(x, 0);
                    Bitmap bitmap5 = new Bitmap(0x11, 0x11);
                    Graphics graphics4 = Graphics.FromImage(bitmap5);
                    graphics4.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics4.FillEllipse(new SolidBrush(Color.FromArgb(120, 120, 110)), new Rectangle(1, 1, 15, 15));
                    graphics4.DrawEllipse(new Pen(Color.FromArgb(60, 60, 0x37), 1f), new Rectangle(1, 1, 15, 15));
                    graphics4.DrawRectangle(new Pen(Color.FromArgb(60, 60, 0x37), 1f), new Rectangle(6, 6, 5, 5));
                    this.maximize.Image = bitmap5;
                    this.maximize.Click += new EventHandler(this.Maximize_Click);
                    this.maximize.SizeMode = PictureBoxSizeMode.CenterImage;
                    panel5.Controls.Add(this.maximize);
                    this.maximize.BringToFront();
                }
                if (this.showMaximize)
                {
                    this.minimize.BackColor = Color.Transparent;
                    this.minimize.Parent = panel5;
                    this.minimize.Size = new Size(0x19, 30);
                    this.minimize.Location = new Point(0x19, 0);
                    Bitmap bitmap6 = new Bitmap(0x11, 0x11);
                    Graphics graphics5 = Graphics.FromImage(bitmap6);
                    graphics5.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics5.FillEllipse(new SolidBrush(Color.FromArgb(120, 120, 110)), new Rectangle(1, 1, 15, 15));
                    graphics5.DrawEllipse(new Pen(Color.FromArgb(60, 60, 0x37), 1f), new Rectangle(1, 1, 15, 15));
                    graphics5.DrawLine(new Pen(Color.FromArgb(60, 60, 0x37), 1f), 6, 9, 11, 9);
                    this.minimize.Image = bitmap6;
                    this.minimize.Click += new EventHandler(this.Minimize_Click);
                    this.minimize.SizeMode = PictureBoxSizeMode.CenterImage;
                    panel5.Controls.Add(this.minimize);
                    this.minimize.BringToFront();
                }
                this.exit.BackColor = Color.Transparent;
                this.exit.Parent = panel5;
                this.exit.Size = new Size(0x19, 30);
                this.exit.Location = new Point(0, 0);
                Bitmap image = new Bitmap(0x11, 0x11);
                Graphics graphics6 = Graphics.FromImage(image);
                graphics6.SmoothingMode = SmoothingMode.AntiAlias;
                graphics6.FillEllipse(new SolidBrush(Color.FromArgb(230, 0x5f, 50)), new Rectangle(1, 1, 15, 15));
                graphics6.DrawEllipse(new Pen(Color.FromArgb(60, 60, 0x37), 1f), new Rectangle(1, 1, 15, 15));
                graphics6.DrawLine(new Pen(Color.FromArgb(60, 60, 0x37), 1f), 6, 6, 11, 11);
                graphics6.DrawLine(new Pen(Color.FromArgb(60, 60, 0x37), 1f), 6, 11, 11, 6);
                this.exit.Image = image;
                this.exit.Click += new EventHandler(this.Exit_Click);
                this.exit.SizeMode = PictureBoxSizeMode.CenterImage;
                panel5.Controls.Add(this.exit);
                this.exit.BringToFront();
                this.WorkPanel.Location = new Point(0, 50);
                if (!this.SetBG)
                {
                    this.BackColor = Color.FromArgb(240, 240, 240);
                    this.SetBG = true;
                }
                this.WorkPanel.BackColor = this.BackColor;
                handle.HandleControl = label;
                handle.HandleControl = panel5;
                this.WorkPanel.Dock = DockStyle.Fill;
                base.Controls.Add(this.WorkPanel);
                base.Controls.Add(panel5);
            }
            base.ResumeLayout(true);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The working area"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel WorkingArea =>
            this.WorkPanel;

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
                            this.DefaultForm = (Form)rootComponent;
                            this.DefaultStyle = ((Form)rootComponent).FormBorderStyle;
                            ((Form)rootComponent).FormBorderStyle = FormBorderStyle.None;
                        }
                    }
                }
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The titles text")]
        public string TitleText
        {
            get =>
                this.titleText;
            set
            {
                this.titleText = value;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the maximize option")]
        public bool ShowMaximize
        {
            get =>
                this.showMaximize;
            set
            {
                this.showMaximize = value;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Exit appliction? otherwise form will just be closed")]
        public bool ExitApplication
        {
            get =>
                this.exitApplication;
            set
            {
                this.exitApplication = value;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the minimize option")]
        public bool ShowMinimize
        {
            get =>
                this.showMinimize;
            set
            {
                this.showMinimize = value;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The forecolor of the material titlebar")]
        public Color MaterialForeColor
        {
            get =>
                this.materialForeColor;
            set
            {
                this.materialForeColor = value;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The forecolor of the material titlebar")]
        public Color MaterialBackColor
        {
            get =>
                this.materialBackColor;
            set
            {
                this.materialBackColor = value;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The forms style")]
        public Style FormStyle
        {
            get =>
                this.formStyle;
            set
            {
                this.formStyle = value;
                this.SetBG = false;
                base.Controls.Clear();
                this.RefreshUI();
            }
        }

        // Nested Types
        public enum Style
        {
            Material,
            MacOS,
            Ubuntu
        }
    }


}
