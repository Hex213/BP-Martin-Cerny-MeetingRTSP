using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUISplashScreen : Component
    {
        // Fields
        private bool allowDragging = true;
        private bool showProgressBar = true;
        private bool isEllipse;
        private int ellipseCornerRadius = 10;
        private XUIFlatProgressBar.Style progressBarStyle = XUIFlatProgressBar.Style.Material;
        private Color loadedColor = Color.DodgerBlue;
        private Color unloadedColor = Color.FromArgb(30, 30, 30);
        private int secondsDisplayed = 0xbb8;
        private Color backColor = Color.FromArgb(30, 30, 30);
        private Size splashSize = new Size(450, 280);
        private Color topTextColor = Color.White;
        private string topText = "VisualStudio";
        private int topTextSize = 0x24;
        private Color bottomTextColor = Color.White;
        private string bottomText = "Community edition";
        private int bottomTextSize = 0x10;
        private Form splashForm = new Form();
        private XUIFlatProgressBar progressBar = new XUIFlatProgressBar();
        private Timer updateProgress = new Timer();
        private Panel background = new Panel();
        private Label text1 = new Label();
        private XUIFormHandle handle = new XUIFormHandle();
        private XUIFormHandle handle2 = new XUIFormHandle();
        private XUIFormHandle handle3 = new XUIFormHandle();
        private XUIFormHandle handle4 = new XUIFormHandle();
        private XUIObjectEllipse ellipse = new XUIObjectEllipse();
        private Label text2 = new Label();
        private Control baseForm;

        // Methods
        public void initializeLoader(Control mainForm)
        {
            this.baseForm = mainForm;
            ((Form)this.baseForm).WindowState = FormWindowState.Minimized;
            ((Form)this.baseForm).ShowInTaskbar = false;
            this.splashForm.BackColor = this.backColor;
            this.splashForm.FormBorderStyle = FormBorderStyle.None;
            this.splashForm.StartPosition = FormStartPosition.CenterScreen;
            this.splashForm.Size = this.splashSize;
            this.background.Dock = DockStyle.Fill;
            this.background.BackColor = this.backColor;
            this.splashForm.Controls.Add(this.background);
            this.progressBar.BarStyle = this.progressBarStyle;
            this.progressBar.InocmpletedColor = this.unloadedColor;
            this.progressBar.CompleteColor = this.loadedColor;
            this.progressBar.Value = 0;
            this.progressBar.Size = new Size(this.splashForm.Width, 10);
            this.progressBar.Location = new Point(0, (this.splashForm.Height / 5) * 4);
            if (!this.showProgressBar)
            {
                this.progressBar.Visible = false;
            }
            this.background.Controls.Add(this.progressBar);
            this.updateProgress.Interval = this.secondsDisplayed / 100;
            this.updateProgress.Tick += new EventHandler(this.updateLoader);
            this.text1.ForeColor = this.topTextColor;
            this.text1.Font = new Font("Ariel", (float)this.topTextSize);
            this.text1.Text = this.topText;
            this.text1.BackColor = this.backColor;
            this.text1.AutoSize = true;
            this.text1.Location = new Point(0, this.splashForm.Height / 4);
            this.background.Controls.Add(this.text1);
            this.text2.ForeColor = this.bottomTextColor;
            this.text2.Font = new Font("Ariel", (float)this.bottomTextSize);
            this.text2.Text = this.bottomText;
            this.text2.BackColor = this.backColor;
            this.text2.AutoSize = true;
            this.text2.Location = new Point((this.text1.Width / 2) - this.text2.Width, this.text1.Location.Y + this.text1.Height);
            this.background.Controls.Add(this.text2);
            this.handle.DockAtTop = false;
            if (this.allowDragging)
            {
                this.handle.HandleControl = this.background;
                this.handle2.HandleControl = this.text1;
                this.handle3.HandleControl = this.text2;
                this.handle4.HandleControl = this.progressBar;
            }
            if (this.isEllipse)
            {
                this.ellipse.CornerRadius = this.ellipseCornerRadius;
                this.ellipse.EffectedForm = this.splashForm;
            }
            this.splashForm.Show();
            this.splashForm.BringToFront();
            this.updateProgress.Enabled = true;
        }

        private void updateLoader(object sender, EventArgs e)
        {
            if (this.progressBar.Value < 100)
            {
                this.progressBar.Value++;
            }
            else
            {
                this.progressBar.Dispose();
                this.updateProgress.Dispose();
                this.background.Dispose();
                this.text1.Dispose();
                this.text2.Dispose();
                this.handle.Dispose();
                this.handle2.Dispose();
                this.handle3.Dispose();
                this.handle4.Dispose();
                this.ellipse.Dispose();
                this.splashForm.Dispose();
                ((Form)this.baseForm).ShowInTaskbar = true;
                ((Form)this.baseForm).WindowState = FormWindowState.Normal;
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Allow dragging the splash")]
        public bool AllowDragging
        {
            get =>
                this.allowDragging;
            set =>
                this.allowDragging = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Show progressbar")]
        public bool ShowProgressBar
        {
            get =>
                this.showProgressBar;
            set =>
                this.showProgressBar = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Is the splash elliptical")]
        public bool IsEllipse
        {
            get =>
                this.isEllipse;
            set =>
                this.isEllipse = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The corner radius if ellipse")]
        public int EllipseCornerRadius
        {
            get =>
                this.ellipseCornerRadius;
            set =>
                this.ellipseCornerRadius = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Progressbar style")]
        public XUIFlatProgressBar.Style ProgressBarStyle
        {
            get =>
                this.progressBarStyle;
            set =>
                this.progressBarStyle = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The progressbar loaded color")]
        public Color LoadedColor
        {
            get =>
                this.loadedColor;
            set =>
                this.loadedColor = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The progressbar unloaded color")]
        public Color UnloadedColor
        {
            get =>
                this.unloadedColor;
            set =>
                this.unloadedColor = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Amount of seconds splash is displayed for in milliseconds")]
        public int SecondsDisplayed
        {
            get =>
                this.secondsDisplayed;
            set =>
                this.secondsDisplayed = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The splash BckColor")]
        public Color BackColor
        {
            get =>
                this.backColor;
            set =>
                this.backColor = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The splash size")]
        public Size SplashSize
        {
            get =>
                this.splashSize;
            set =>
                this.splashSize = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The top text color")]
        public Color TopTextColor
        {
            get =>
                this.topTextColor;
            set =>
                this.topTextColor = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The top text")]
        public string TopText
        {
            get =>
                this.topText;
            set =>
                this.topText = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The top text size")]
        public int TopTextSize
        {
            get =>
                this.topTextSize;
            set =>
                this.topTextSize = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The bottom text color")]
        public Color BottomTextColor
        {
            get =>
                this.bottomTextColor;
            set =>
                this.bottomTextColor = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The bottom text")]
        public string BottomText
        {
            get =>
                this.bottomText;
            set =>
                this.bottomText = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The bottom text size")]
        public int BottomTextSize
        {
            get =>
                this.bottomTextSize;
            set =>
                this.bottomTextSize = value;
        }
    }
}
