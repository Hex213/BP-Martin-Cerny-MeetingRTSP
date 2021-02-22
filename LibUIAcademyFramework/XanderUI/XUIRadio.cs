using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIRadio : Control
    {
        // Fields
        private bool isChecked;
        private Color radioColor = Color.FromArgb(0, 0xa2, 250);
        private Color radioHoverColor = Color.FromArgb(0xf9, 0x37, 0x62);
        private Style radioStyle = Style.Material;
        private Color currentColor;

        // Methods
        public XUIRadio()
        {
            base.Size = new Size(100, 0x10);
            this.Text = base.Name;
            this.ForeColor = Color.White;
            this.currentColor = this.radioColor;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            foreach (Control control in base.Parent.Controls)
            {
                if (control is RadioButton)
                {
                    ((RadioButton)control).Checked = false;
                }
                if (control is XUIRadio)
                {
                    ((XUIRadio)control).Checked = false;
                }
            }
            this.isChecked = true;
            base.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.currentColor = this.radioHoverColor;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.currentColor = this.radioColor;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (this.radioStyle == Style.Material)
            {
                e.Graphics.DrawEllipse(new Pen(this.currentColor, 2f), 2, 2, base.Height - 4, base.Height - 4);
                if (this.isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(this.currentColor), new Rectangle(5, 5, (base.Height - 2) - 8, (base.Height - 2) - 8), 0f, 360f);
                }
                e.Graphics.FillPie(new SolidBrush(this.currentColor), new Rectangle(1, 1, base.Height - 2, base.Height - 2), 0f, 360f);
                if (this.isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.White), new Rectangle(4, 4, (base.Height - 2) - 6, (base.Height - 2) - 6), 0f, 360f);
                }
            }
            if (this.radioStyle == Style.iOS)
            {
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(30, 150, 240), 2f), 2, 2, base.Height - 4, base.Height - 4);
                if (this.isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(30, 150, 240)), new Rectangle(5, 5, (base.Height - 2) - 8, (base.Height - 2) - 8), 0f, 360f);
                }
            }
            if (this.radioStyle == Style.Android)
            {
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(0, 150, 0x87), 2f), 2, 2, base.Height - 4, base.Height - 4);
                if (this.isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(0, 150, 0x87)), new Rectangle(5, 5, (base.Height - 2) - 8, (base.Height - 2) - 8), 0f, 360f);
                }
            }
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };
            SolidBrush brush = new SolidBrush(this.ForeColor);
            RectangleF layoutRectangle = new RectangleF((float)(base.Height + 3), 0f, (float)((base.Width - base.Height) - 2), (float)base.Height);
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString(this.Text, this.Font, brush, layoutRectangle, format);
            base.OnPaint(e);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Checked or unchecked")]
        public bool Checked
        {
            get =>
                this.isChecked;
            set
            {
                this.isChecked = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Radio color")]
        public Color RadioColor
        {
            get =>
                this.radioColor;
            set
            {
                this.radioColor = value;
                this.currentColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Radio color when hovering")]
        public Color RadioHoverColor
        {
            get =>
                this.radioHoverColor;
            set
            {
                this.radioHoverColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The radio style")]
        public Style RadioStyle
        {
            get =>
                this.radioStyle;
            set
            {
                this.radioStyle = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum Style
        {
            iOS,
            Android,
            Material
        }
    }



}
