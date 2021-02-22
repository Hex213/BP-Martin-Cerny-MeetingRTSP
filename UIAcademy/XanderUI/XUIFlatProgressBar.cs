using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIFlatProgressBar : Control
    {
        // Fields
        private BufferedGraphics bufferedGraphics;
        private Style barStyle = Style.Material;
        private int value = 50;
        private Color completeColor = Color.FromArgb(1, 0x77, 0xd7);
        private Color borderColor = Color.Black;
        private bool showBorder = true;
        private Color incompletedColor = Color.White;
        private int maxValue = 100;

        // Methods
        public XUIFlatProgressBar()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.Size = new Size(300, 5);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            BufferedGraphicsContext current = BufferedGraphicsManager.Current;
            current.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), base.ClientRectangle);
            this.bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            this.bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            this.bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.bufferedGraphics.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.bufferedGraphics.Graphics.Clear(this.BackColor);
            if (this.barStyle == Style.Flat)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(this.incompletedColor), 0, 0, base.Width, base.Height);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(this.completeColor), 0, 0, (this.value * base.Width) / this.maxValue, base.Height);
            }
            if (this.barStyle == Style.IOS)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, 180, 180)), 0, 0, base.Width, base.Height);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 120, 250)), 0, 0, (this.value * base.Width) / this.maxValue, base.Height);
            }
            if (this.barStyle == Style.Material)
            {
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), Color.Black, Color.Black, 0f, false);
                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
                blend.Colors = new Color[] { Color.FromArgb(0x4c, 0xd9, 100), Color.FromArgb(0x55, 0xcd, 0xcd), Color.FromArgb(2, 0x7c, 0xff), Color.FromArgb(130, 0x4b, 180), Color.FromArgb(0xff, 0, 150), Color.FromArgb(0xff, 0x2d, 0x55) };
                brush.InterpolationColors = blend;
                brush.RotateTransform(1f);
                this.bufferedGraphics.Graphics.FillRectangle(brush, new Rectangle(0, 0, base.Width, base.Height));
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(this.incompletedColor), (this.value * base.Width) / this.maxValue, 0, base.Width - ((this.value * base.Width) / this.maxValue), base.Height);
            }
            if (this.ShowBorder)
            {
                this.bufferedGraphics.Graphics.DrawRectangle(new Pen(this.BorderColor, 1f), new Rectangle(1, 1, base.Width - 2, base.Height - 2));
            }
            this.bufferedGraphics.Render(e.Graphics);
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.Invalidate();
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The progress bar style")]
        public Style BarStyle
        {
            get =>
                this.barStyle;
            set
            {
                this.barStyle = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The progress value")]
        public int Value
        {
            get =>
                this.value;
            set
            {
                this.value = value;
                if (this.value < 0)
                {
                    this.value = 0;
                }
                if (this.value > this.maxValue)
                {
                    this.value = this.maxValue;
                }
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The progress complete color")]
        public Color CompleteColor
        {
            get =>
                this.completeColor;
            set
            {
                this.completeColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The progress bar border color")]
        public Color BorderColor
        {
            get =>
                this.borderColor;
            set
            {
                this.borderColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the progress bar border")]
        public bool ShowBorder
        {
            get =>
                this.showBorder;
            set
            {
                this.showBorder = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The progress incompleted color")]
        public Color InocmpletedColor
        {
            get =>
                this.incompletedColor;
            set
            {
                this.incompletedColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The maximum value")]
        public int MaxValue
        {
            get =>
                this.maxValue;
            set
            {
                this.maxValue = value;
                if (this.Value > this.maxValue)
                {
                    this.Value = this.maxValue;
                }
                base.Invalidate();
            }
        }

        // Nested Types
        public enum Style
        {
            Flat,
            Material,
            IOS
        }
    }



}
