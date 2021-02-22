using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUICircleProgressBar : Control
    {
        // Fields
        private BufferedGraphics bufferedGraphics;
        private Timer updateUI = new Timer();
        private int StartPoint = 270;
        private Color unFilledColor = Color.FromArgb(0x72, 0x72, 0x72);
        private Color filledColor = Color.FromArgb(60, 220, 210);
        private int filledColorAlpha = 130;
        private int unfilledThickness = 0x18;
        private int filledThickness = 40;
        public int percentage = 0x3f;
        public int animationSpeed = 5;
        public bool isAnimated;
        public int textSize = 0x19;
        public Color textColor = Color.Gray;
        public bool showText = true;

        // Events
        public event EventHandler PercentageChanged;

        // Methods
        public XUICircleProgressBar()
        {
            base.Size = new Size(200, 200);
            this.updateUI.Tick += new EventHandler(this.Animate);
            this.updateUI.Interval = 200 / this.animationSpeed;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        private void Animate(object sender, EventArgs e)
        {
            if (this.StartPoint == 360)
            {
                this.StartPoint = 0;
            }
            this.StartPoint += this.animationSpeed;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            BufferedGraphicsContext current = BufferedGraphicsManager.Current;
            current.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), new Rectangle(0, 0, 1, 1));
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), base.ClientRectangle);
            this.bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            this.bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.bufferedGraphics.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (this.BackgroundImage == null)
            {
                this.bufferedGraphics.Graphics.Clear(this.BackColor);
            }
            else
            {
                this.bufferedGraphics.Graphics.DrawImage(this.BackgroundImage, 0, 0);
            }
            Rectangle rect = new Rectangle((this.filledThickness / 2) + 1, (this.filledThickness / 2) + 1, (base.Width - this.filledThickness) - 2, (base.Height - this.filledThickness) - 2);
            this.bufferedGraphics.Graphics.DrawArc(new Pen(this.unFilledColor, (float)this.unfilledThickness), rect, (float)this.StartPoint, 360f);
            this.bufferedGraphics.Graphics.DrawArc(new Pen(Color.FromArgb(this.filledColorAlpha, this.filledColor.R, this.filledColor.G, this.filledColor.B), (float)this.filledThickness), rect, (float)this.StartPoint, (float)((int)(this.Percentage * 3.6)));
            if (this.ShowText)
            {
                Rectangle layoutRectangle = new Rectangle(0, 0, base.Width, base.Height);
                StringFormat format = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                this.bufferedGraphics.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                this.bufferedGraphics.Graphics.DrawString(this.Percentage.ToString() + "%", new Font("Ariel", (float)this.textSize), new SolidBrush(this.textColor), layoutRectangle, format);
            }
            this.bufferedGraphics.Render(e.Graphics);
            base.OnPaint(e);
        }

        protected virtual void OnPercentageChanged()
        {
            if (this.PercentageChanged != null)
            {
                this.PercentageChanged(this, EventArgs.Empty);
            }
        }

        // Properties
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color ForeColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string Text { get; set; }

        [Category("XanderUI"), Browsable(true), Description("Unfilled circle color")]
        public Color UnFilledColor
        {
            get =>
                this.unFilledColor;
            set
            {
                this.unFilledColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Filled circle color")]
        public Color FilledColor
        {
            get =>
                this.filledColor;
            set
            {
                this.filledColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Filled colors alpha value")]
        public int FilledColorAlpha
        {
            get =>
                this.filledColorAlpha;
            set
            {
                this.filledColorAlpha = value;
                if (value > 0xff)
                {
                    this.filledColorAlpha = 0xff;
                }
                if (value < 1)
                {
                    this.filledColorAlpha = 1;
                }
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Unfilled circle thickness")]
        public int UnfilledThickness
        {
            get =>
                this.unfilledThickness;
            set
            {
                this.unfilledThickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Unfilled circle thickness")]
        public int FilledThickness
        {
            get =>
                this.filledThickness;
            set
            {
                this.filledThickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The progress circle percentage")]
        public int Percentage
        {
            get =>
                this.percentage;
            set
            {
                this.percentage = value;
                if (value < 0)
                {
                    this.percentage = 0;
                }
                if (value > 100)
                {
                    this.percentage = 100;
                }
                this.OnPercentageChanged();
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The animation speed")]
        public int AnimationSpeed
        {
            get =>
                this.animationSpeed;
            set
            {
                this.animationSpeed = value;
                if (value < 1)
                {
                    this.animationSpeed = 1;
                }
                if (this.animationSpeed > 10)
                {
                    this.animationSpeed = 10;
                }
                this.updateUI.Interval = 200 / this.animationSpeed;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Is the control animated")]
        public bool IsAnimated
        {
            get =>
                this.isAnimated;
            set
            {
                this.isAnimated = value;
                this.updateUI.Enabled = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text size")]
        public int TextSize
        {
            get =>
                this.textSize;
            set
            {
                this.textSize = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Text color")]
        public Color TextColor
        {
            get =>
                this.textColor;
            set
            {
                this.textColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show text")]
        public bool ShowText
        {
            get =>
                this.showText;
            set
            {
                this.showText = value;
                base.Invalidate();
            }
        }
    }



}
