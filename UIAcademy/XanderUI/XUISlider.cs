using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUISlider : Control
    {
        // Fields
        private Rectangle barRectangle;
        private BufferedGraphics bufferedGraphics;
        private bool onHandle;
        private int barThickness = 4;
        private int bigStepIncrement = 10;
        private int percentage = 50;
        private Color filledColor = Color.FromArgb(1, 0x77, 0xd7);
        private Color unfilledColor = Color.FromArgb(0x1a, 0xa9, 0xdb);
        private Color knobColor = Color.Gray;
        private Image knobImage;
        private bool quickHopping;
        private Style sliderStyle = Style.Windows10;

        // Events
        public event EventHandler Scroll;

        // Methods
        public XUISlider()
        {
            base.Size = new Size(250, 20);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.barRectangle = new Rectangle((base.Height / 2) + 1, 1, base.Width - base.Height, base.Height - 1);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.quickHopping)
            {
                this.Percentage = (int)Math.Round((double)(((double)(100 * e.X)) / ((double)base.Width)));
                this.onHandle = true;
            }
            else
            {
                int num = (this.Percentage * base.Width) / 100;
                if ((e.X > (num - (base.Height / 2))) && (e.X < (num + (base.Height / 2))))
                {
                    this.onHandle = true;
                }
                else if (e.X < (num - (base.Height / 2)))
                {
                    this.Percentage -= this.bigStepIncrement;
                    if (this.Percentage < 0)
                    {
                        this.Percentage = 0;
                    }
                    base.Invalidate();
                }
                else if (e.X > (num + (base.Height / 2)))
                {
                    this.Percentage += this.bigStepIncrement;
                    if (this.Percentage > 100)
                    {
                        this.Percentage = 100;
                    }
                    base.Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.onHandle)
            {
                this.Percentage = (int)Math.Round((double)(((double)(100 * e.X)) / ((double)base.Width)));
                if (this.Percentage < 0)
                {
                    this.Percentage = 0;
                }
                if (this.Percentage > 100)
                {
                    this.Percentage = 100;
                }
                base.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.onHandle = false;
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
            int num1 = (this.Percentage * base.Width) / 100;
            int num = (this.Percentage * this.barRectangle.Width) / 100;
            this.bufferedGraphics.Graphics.Clear(this.BackColor);
            if (this.sliderStyle == Style.Flat)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(this.unfilledColor), (base.Height / 2) + 1, (base.Height / 2) - (this.barThickness / 2), (base.Width - base.Height) - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(this.filledColor), 1 + (base.Height / 2), (base.Height / 2) - (this.barThickness / 2), num - 2, this.barThickness);
                if (this.knobImage == null)
                {
                    this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(this.knobColor), num + 1, 1, base.Height - 2, base.Height - 2);
                }
                else
                {
                    this.bufferedGraphics.Graphics.DrawImage(new Bitmap(this.knobImage, base.Height - 2, base.Height - 2), num + 1, 1);
                }
            }
            if (this.sliderStyle == Style.MacOS)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0xb9, 0xb9, 0xb9)), (base.Height / 2) + 1, (base.Height / 2) - (this.barThickness / 2), (base.Width - base.Height) - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(80, 150, 230)), 1 + (base.Height / 2), (base.Height / 2) - (this.barThickness / 2), num - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(Color.White), num + 1, 1, base.Height - 2, base.Height - 2);
                this.bufferedGraphics.Graphics.DrawEllipse(new Pen(Color.FromArgb(190, 200, 200)), num + 1, 1, base.Height - 2, base.Height - 2);
            }
            if (this.sliderStyle == Style.Windows10)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x92, 0x93, 0x94)), (base.Height / 2) + 1, (base.Height / 2) - (this.barThickness / 2), (base.Width - base.Height) - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x41, 0x9b, 0xe1)), 1 + (base.Height / 2), (base.Height / 2) - (this.barThickness / 2), num - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 120, 0xd7)), (num + 1) + (base.Height / 3), 3, (base.Height / 2) - 2, base.Height - 6);
                this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 120, 0xd7)), (num + 1) + (base.Height / 3), 0, (base.Height / 2) - 2, 4);
                this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 120, 0xd7)), (num + 1) + (base.Height / 3), base.Height - 5, (base.Height / 2) - 2, 4);
            }
            if (this.sliderStyle == Style.Android)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), (base.Height / 2) + 1, (base.Height / 2) - (this.barThickness / 2), (base.Width - base.Height) - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 180, 230)), 1 + (base.Height / 2), (base.Height / 2) - (this.barThickness / 2), num - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(50, 180, 230)), (int)((num + 1) + ((this.barThickness / 3) * 5)), (int)((base.Height / 2) - ((this.barThickness / 3) * 4)), (int)(this.barThickness * 2), (int)(this.barThickness * 2));
                this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(100, 50, 180, 230)), num + 1, 1, base.Height - 2, base.Height - 2);
                this.bufferedGraphics.Graphics.DrawEllipse(new Pen(Color.FromArgb(50, 180, 230), 2f), num + 1, 1, base.Height - 2, base.Height - 2);
            }
            if (this.sliderStyle == Style.Material)
            {
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), Color.Black, Color.Black, 0f, false);
                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
                blend.Colors = new Color[] { Color.FromArgb(0x4c, 0xd9, 100), Color.FromArgb(0x55, 0xcd, 0xcd), Color.FromArgb(2, 0x7c, 0xff), Color.FromArgb(130, 0x4b, 180), Color.FromArgb(0xff, 0, 150), Color.FromArgb(0xff, 0x2d, 0x55) };
                brush.InterpolationColors = blend;
                brush.RotateTransform(1f);
                this.bufferedGraphics.Graphics.FillRectangle(brush, (base.Height / 2) + 1, (base.Height / 2) - (this.barThickness / 2), (base.Width - base.Height) - 2, this.barThickness);
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.LightGray), (1 + (base.Height / 2)) + num, (base.Height / 2) - (this.barThickness / 2), ((base.Width - base.Height) - 2) - num, this.barThickness);
                this.bufferedGraphics.Graphics.FillEllipse(new SolidBrush(Color.White), num + 1, 1, base.Height - 2, base.Height - 2);
                this.bufferedGraphics.Graphics.DrawEllipse(new Pen(Color.FromArgb(200, 200, 200)), num + 1, 1, base.Height - 2, base.Height - 2);
            }
            this.bufferedGraphics.Render(e.Graphics);
        }

        protected virtual void OnScroll()
        {
            if (this.Scroll != null)
            {
                this.Scroll(this, EventArgs.Empty);
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The bar thickness")]
        public int BarThickness
        {
            get =>
                this.barThickness;
            set
            {
                this.barThickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The increment incresed or decreased when not clicking in the handle")]
        public int BigStepIncrement
        {
            get =>
                this.bigStepIncrement;
            set
            {
                this.bigStepIncrement = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The default percentage")]
        public int Percentage
        {
            get =>
                this.percentage;
            set
            {
                this.percentage = value;
                this.OnScroll();
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The filled color")]
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

        [Category("XanderUI"), Browsable(true), Description("The unfilled color")]
        public Color UnfilledColor
        {
            get =>
                this.unfilledColor;
            set
            {
                this.unfilledColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The knob color")]
        public Color KnobColor
        {
            get =>
                this.knobColor;
            set
            {
                this.knobColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The knob image")]
        public Image KnobImage
        {
            get =>
                this.knobImage;
            set
            {
                this.knobImage = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Allow instantly jumping to the position clicked")]
        public bool QuickHopping
        {
            get =>
                this.quickHopping;
            set
            {
                this.quickHopping = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The slider style")]
        public Style SliderStyle
        {
            get =>
                this.sliderStyle;
            set
            {
                this.sliderStyle = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum Style
        {
            Flat,
            Material,
            MacOS,
            Android,
            Windows10
        }
    }


}
