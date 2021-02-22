using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIGauge : Control
    {
        // Fields
        public Style gaugeStyle = Style.Material;
        public int thickness = 8;
        public int dialThickness = 5;
        public int percentage = 0x4b;
        public Color dialColor = Color.Gray;
        public Color unfilledColor = Color.Gray;
        public Color filledColor = Color.FromArgb(0, 0xa2, 250);

        // Methods
        public XUIGauge()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Size = new Size(140, 70);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (this.gaugeStyle == Style.Standard)
            {
                Rectangle rect = new Rectangle(1 + (this.thickness / 2), 1 + (this.thickness / 2), (base.Width - 2) - this.thickness, (base.Height * 2) - this.thickness);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(0xff, 220, 0), (float)(this.thickness / 4)), rect, 180f, 0x2cf);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(0xff, 150, 0), (float)((this.thickness / 4) * 2)), rect, 0xe2f, 0x2cf);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(250, 90, 0), (float)((this.thickness / 4) * 3)), rect, 0x110f, 0x2cf);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(0xff, 0, 0), (float)this.thickness), rect, 0x13ef, 0x2cf);
                rect.Inflate(0 - this.thickness, 0 - this.thickness);
                e.Graphics.FillPie(new SolidBrush(this.dialColor), new Rectangle((base.Width / 2) - this.thickness, base.Height - this.thickness, this.thickness * 2, this.thickness * 2), 0f, 360f);
                if (this.percentage < 5)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.Black), rect, (float)((180 + (this.dialThickness * 2)) - 2), (float)this.dialThickness);
                }
                else if (this.percentage > 0x5f)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.Black), rect, (float)(360 - (this.dialThickness * 2)), (float)this.dialThickness);
                }
                else
                {
                    e.Graphics.FillPie(new SolidBrush(this.dialColor), rect, (float)((180 + ((int)(this.percentage * 1.8))) - (this.dialThickness / 2)), (float)this.dialThickness);
                }
            }
            if (this.gaugeStyle == Style.Material)
            {
                Rectangle rect = new Rectangle(1 + (this.thickness / 2), 1 + (this.thickness / 2), (base.Width - 2) - this.thickness, (base.Height * 2) - this.thickness);
                e.Graphics.DrawArc(new Pen(new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), Color.FromArgb(0xf9, 0x37, 0x62), Color.FromArgb(0, 0xa2, 250), 1f), (float)this.thickness), rect, 180f, (float)(((int)(this.percentage * 1.8)) - 1));
                e.Graphics.DrawArc(new Pen(Color.FromArgb(0, 0xa2, 250), (float)this.thickness), rect, (float)((180 + ((int)(this.percentage * 1.8))) + 1), (float)((180 - ((int)(this.percentage * 1.8))) + 5));
            }
            if (this.gaugeStyle == Style.Flat)
            {
                Rectangle rect = new Rectangle(1 + (this.thickness / 2), 1 + (this.thickness / 2), (base.Width - 2) - this.thickness, (base.Height * 2) - this.thickness);
                e.Graphics.DrawArc(new Pen(this.filledColor, (float)this.thickness), rect, 180f, (float)(((int)(this.percentage * 1.8)) - 1));
                e.Graphics.DrawArc(new Pen(this.unfilledColor, (float)this.thickness), rect, (float)((180 + ((int)(this.percentage * 1.8))) + 1), (float)((180 - ((int)(this.percentage * 1.8))) + 5));
            }
            base.OnPaint(e);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The gauge style")]
        public Style GaugeStyle
        {
            get =>
                this.gaugeStyle;
            set
            {
                this.gaugeStyle = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The gauge thickness")]
        public int Thickness
        {
            get =>
                this.thickness;
            set
            {
                this.thickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The gauge dial thickness")]
        public int DialThickness
        {
            get =>
                this.dialThickness;
            set
            {
                this.dialThickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The gauge percentage")]
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
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The gauge percentage")]
        public Color DialColor
        {
            get =>
                this.dialColor;
            set
            {
                this.dialColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The gauge unfilled color")]
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

        [Category("XanderUI"), Browsable(true), Description("The gauge filled color")]
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

        // Nested Types
        public enum Style
        {
            Standard,
            Material,
            Flat
        }
    }


}
