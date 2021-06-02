using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUICheckBox : Control
    {
        // Fields
        private bool isChecked;
        private int tickThickness = 2;
        private Color checkboxColor = Color.FromArgb(0, 0xa2, 250);
        private Color checkboxCheckColor = Color.White;
        private Color checkboxHoverColor = Color.FromArgb(0xf9, 0x37, 0x62);
        private Style checkboxStyle = Style.Material;
        private Color currentColor;

        // Events
        public event EventHandler CheckedStateChanged;

        // Methods
        public XUICheckBox()
        {
            base.Size = new Size(100, 20);
            this.Text = base.Name;
            this.ForeColor = Color.White;
            this.currentColor = this.checkboxColor;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!this.Checked)
            {
                this.Checked = true;
            }
            else
            {
                this.Checked = false;
            }
        }

        protected virtual void OnCheckedStateChanged()
        {
            if (this.CheckedStateChanged != null)
            {
                this.CheckedStateChanged(this, new EventArgs());
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.currentColor = this.checkboxHoverColor;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.currentColor = this.checkboxColor;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (this.checkboxStyle == Style.Material)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.currentColor), 1, 1, base.Height - 2, base.Height - 2);
                if (this.isChecked)
                {
                    e.Graphics.DrawLine(new Pen(this.checkboxCheckColor, (float)this.tickThickness), 2, (base.Height / 3) * 2, base.Height / 2, base.Height - 2);
                    e.Graphics.DrawLine(new Pen(this.checkboxCheckColor, (float)this.tickThickness), base.Height / 2, base.Height - 2, base.Height - 2, 1);
                }
            }
            if (this.checkboxStyle == Style.iOS)
            {
                if (!this.isChecked)
                {
                    e.Graphics.DrawEllipse(new Pen(Color.FromArgb(200, 200, 200)), 2, 2, base.Height - 4, base.Height - 4);
                }
                if (this.isChecked)
                {
                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 120, 0xff)), 1, 1, base.Height - 2, base.Height - 2);
                    e.Graphics.DrawLine(new Pen(Color.White, (float)this.tickThickness), (int)(base.Height / 5), (int)(base.Height / 2), (int)(base.Height / 2), (int)((base.Height / 4) * 3));
                    e.Graphics.DrawLine(new Pen(Color.White, (float)this.tickThickness), (int)(base.Height / 2), (int)((base.Height / 4) * 3), (int)((base.Height / 5) * 4), (int)(base.Height / 4));
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
                this.OnCheckedStateChanged();
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Thickness of the tick when checked")]
        public int TickThickness
        {
            get =>
                this.tickThickness;
            set
            {
                this.tickThickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Checkbox color")]
        public Color CheckboxColor
        {
            get =>
                this.checkboxColor;
            set
            {
                this.checkboxColor = value;
                this.currentColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Checkbox color")]
        public Color CheckboxCheckColor
        {
            get =>
                this.checkboxCheckColor;
            set
            {
                this.checkboxCheckColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Checkbox color when hovering")]
        public Color CheckboxHoverColor
        {
            get =>
                this.checkboxHoverColor;
            set
            {
                this.checkboxHoverColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The Checkbox style")]
        public Style CheckboxStyle
        {
            get =>
                this.checkboxStyle;
            set
            {
                this.checkboxStyle = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum Style
        {
            iOS,
            Material
        }
    }



}
