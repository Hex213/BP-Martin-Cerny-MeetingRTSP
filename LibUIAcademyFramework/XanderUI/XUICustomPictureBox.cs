using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUICustomPictureBox : Control
    {
        // Fields
        private int x;
        private int y;
        private BufferedGraphics bufferedGraphics;
        private bool isElipse;
        private Image image;
        private bool isParallax;
        private bool filterEnabled = true;
        private Color color1 = Color.DodgerBlue;
        private Color color2 = Color.DodgerBlue;
        private int filterAlpha = 200;

        // Methods
        public XUICustomPictureBox()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Size = new Size(150, 150);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.x = 0 - (base.Width / 2);
            this.y = 0 - (base.Height / 2);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.isParallax)
            {
                this.x = e.X - base.Width;
                this.y = e.Y - base.Height;
                base.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphicsContext current = BufferedGraphicsManager.Current;
            current.MaximumBuffer = new Size(base.Width, base.Height);
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), base.ClientRectangle);
            this.bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            this.bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.bufferedGraphics.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.bufferedGraphics.Graphics.Clear(this.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            if (this.image != null)
            {
                if (this.isParallax)
                {
                    if (this.isParallax)
                    {
                        try
                        {
                            this.bufferedGraphics.Graphics.DrawImage(new Bitmap(this.image, base.Width * 2, base.Height * 2), this.x, this.y);
                            this.bufferedGraphics.Render(e.Graphics);
                        }
                        catch
                        {
                        }
                    }
                }
                else if (!this.isElipse)
                {
                    e.Graphics.DrawImage(new Bitmap(this.image, base.Width, base.Height), 0, 0);
                    if (this.filterEnabled)
                    {
                        Brush brush = new LinearGradientBrush(base.ClientRectangle, Color.FromArgb(this.filterAlpha, this.color1), Color.FromArgb(this.filterAlpha, this.color2), 180f);
                        e.Graphics.FillRectangle(brush, 0, 0, base.Width, base.Height);
                    }
                }
                else
                {
                    Brush brush = new TextureBrush(new Bitmap(this.image, base.Width, base.Height), new Rectangle(0, 0, base.Width, base.Height));
                    e.Graphics.FillEllipse(brush, 0, 0, base.Width, base.Height);
                    if (this.filterEnabled)
                    {
                        Brush brush2 = new LinearGradientBrush(base.ClientRectangle, Color.FromArgb(this.filterAlpha, this.color1), Color.FromArgb(this.filterAlpha, this.color2), 180f);
                        e.Graphics.FillEllipse(brush2, 0, 0, base.Width, base.Height);
                    }
                }
            }
        }

        private void updateParallax()
        {
            try
            {
                this.bufferedGraphics.Graphics.Clear(this.BackColor);
                this.bufferedGraphics.Graphics.DrawImage(new Bitmap(this.image, base.Width * 2, base.Height * 2), this.x, this.y);
                this.bufferedGraphics.Render(base.CreateGraphics());
            }
            catch
            {
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Is the image eliptical")]
        public bool IsElipse
        {
            get =>
                this.isElipse;
            set
            {
                this.isElipse = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Is the image eliptical")]
        public Image Image
        {
            get =>
                this.image;
            set
            {
                this.image = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Is the image eliptical")]
        public bool IsParallax
        {
            get =>
                this.isParallax;
            set
            {
                this.isParallax = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Enable filters")]
        public bool FilterEnabled
        {
            get =>
                this.filterEnabled;
            set
            {
                this.filterEnabled = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Filter color 1")]
        public Color Color1
        {
            get =>
                this.color1;
            set
            {
                this.color1 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Filter color 2")]
        public Color Color2
        {
            get =>
                this.color2;
            set
            {
                this.color2 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Filter alpha")]
        public int FilterAlpha
        {
            get =>
                this.filterAlpha;
            set
            {
                this.filterAlpha = value;
                base.Invalidate();
            }
        }
    }



}
