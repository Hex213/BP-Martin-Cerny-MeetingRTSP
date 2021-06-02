using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIGradientPanel : Panel
    {
        // Fields
        private BufferedGraphics bufferedGraphics;
        private Color primerColor = Color.White;
        private Color topLeft = Color.DeepSkyBlue;
        private Color topRight = Color.Fuchsia;
        private Color bottomLeft = Color.Black;
        private Color bottomRight = Color.Fuchsia;
        private GradientStyle style = GradientStyle.Corners;

        // Methods
        public XUIGradientPanel()
        {
            this.DoubleBuffered = true;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;
            base.Size = new Size(200, 200);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphicsContext current = BufferedGraphicsManager.Current;
            current.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), base.ClientRectangle);
            this.bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            this.bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.bufferedGraphics.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.bufferedGraphics.Graphics.Clear(this.primerColor);
            if (this.style != GradientStyle.Corners)
            {
                Brush brush = (this.style != GradientStyle.Vertical) ? new LinearGradientBrush(base.ClientRectangle, this.topLeft, this.topRight, 90f) : new LinearGradientBrush(base.ClientRectangle, this.topLeft, this.topRight, 720f);
                this.bufferedGraphics.Graphics.FillRectangle(brush, base.ClientRectangle);
                brush.Dispose();
            }
            else
            {
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), this.TopLeft, Color.Transparent, 0x2df);
                this.bufferedGraphics.Graphics.FillRectangle(brush, base.ClientRectangle);
                brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), this.topRight, Color.Transparent, 0x87f);
                this.bufferedGraphics.Graphics.FillRectangle(brush, base.ClientRectangle);
                brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), this.bottomRight, Color.Transparent, 0xe1f);
                this.bufferedGraphics.Graphics.FillRectangle(brush, base.ClientRectangle);
                brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), this.bottomLeft, Color.Transparent, 0x13bf);
                this.bufferedGraphics.Graphics.FillRectangle(brush, base.ClientRectangle);
                brush.Dispose();
            }
            this.bufferedGraphics.Render(e.Graphics);
        }

        // Properties
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color BackColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color ForeColor { get; set; }

        [Category("XanderUI"), Browsable(true), Description("The primer color")]
        public Color PrimerColor
        {
            get =>
                this.primerColor;
            set
            {
                this.primerColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The top left color")]
        public Color TopLeft
        {
            get =>
                this.topLeft;
            set
            {
                this.topLeft = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The top right color")]
        public Color TopRight
        {
            get =>
                this.topRight;
            set
            {
                this.topRight = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The bottom left color")]
        public Color BottomLeft
        {
            get =>
                this.bottomLeft;
            set
            {
                this.bottomLeft = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The bottom right color")]
        public Color BottomRight
        {
            get =>
                this.bottomRight;
            set
            {
                this.bottomRight = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The gradient orientation")]
        public GradientStyle Style
        {
            get =>
                this.style;
            set
            {
                this.style = value;
                this.Refresh();
            }
        }

        // Nested Types
        public enum GradientStyle
        {
            Horizontal,
            Vertical,
            Corners
        }
    }


}
