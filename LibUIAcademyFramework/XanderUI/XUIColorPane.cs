using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIColorPane : Control
    {
        // Fields
        private int Increment;
        private Color selectedColor;
        private Color gridColor = Color.White;
        private bool showGrid = true;
        private Color color1 = Color.FromArgb(30, 0x21, 0x26);
        private Color color2 = Color.FromArgb(0x25, 40, 0x31);
        private Color color3 = Color.FromArgb(0x18, 11, 0x38);
        private Color color4 = Color.FromArgb(0x30, 0x24, 0x4c);
        private Color color5 = Color.FromArgb(1, 0x77, 0xd7);
        private Color color6 = Color.FromArgb(0x1a, 0xa9, 0xdb);
        private Color color7 = Color.FromArgb(0x18, 0xca, 0x8e);
        private Color color8 = Color.FromArgb(0x66, 0xd9, 0xae);
        private Color color9 = Color.FromArgb(230, 0x47, 0x59);
        private Color color10 = Color.FromArgb(0xea, 0x81, 0x88);
        private Color color11 = Color.FromArgb(0x9f, 0x85, 0xff);
        private Color color12 = Color.FromArgb(0xbc, 170, 0xfc);
        private Color color13 = Color.FromArgb(0xe4, 0xd8, 0x36);
        private Color color14 = Color.FromArgb(0xeb, 0xe3, 120);

        // Events
        public event EventHandler ColorChanged;

        // Methods
        public XUIColorPane()
        {
            base.Size = new Size(0xaf, 50);
            this.Increment = base.Width / 7;
            this.Cursor = Cursors.Hand;
        }

        protected virtual void OnColorChange()
        {
            if (this.ColorChanged != null)
            {
                this.ColorChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if ((e.X > 0) && (e.Y < base.Height))
            {
                this.selectedColor = this.color2;
            }
            if ((e.X > 0) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color1;
            }
            if ((e.X > this.Increment) && (e.Y < base.Height))
            {
                this.selectedColor = this.color4;
            }
            if ((e.X > this.Increment) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color3;
            }
            if ((e.X > (this.Increment * 2)) && (e.Y < base.Height))
            {
                this.selectedColor = this.color6;
            }
            if ((e.X > (this.Increment * 2)) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color5;
            }
            if ((e.X > (this.Increment * 3)) && (e.Y < base.Height))
            {
                this.selectedColor = this.color8;
            }
            if ((e.X > (this.Increment * 3)) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color7;
            }
            if ((e.X > (this.Increment * 4)) && (e.Y < base.Height))
            {
                this.selectedColor = this.color10;
            }
            if ((e.X > (this.Increment * 4)) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color9;
            }
            if ((e.X > (this.Increment * 5)) && (e.Y < base.Height))
            {
                this.selectedColor = this.color12;
            }
            if ((e.X > (this.Increment * 5)) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color11;
            }
            if ((e.X > (this.Increment * 6)) && (e.Y < base.Height))
            {
                this.selectedColor = this.color14;
            }
            if ((e.X > (this.Increment * 6)) && (e.Y < (base.Height / 2)))
            {
                this.selectedColor = this.color13;
            }
            this.OnColorChange();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.Increment = base.Width / 7;
            e.Graphics.FillRectangle(new SolidBrush(this.color1), 0, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color2), 0, base.Height / 2, this.Increment, base.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.color3), this.Increment, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color4), this.Increment, base.Height / 2, this.Increment, base.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.color5), this.Increment * 2, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color6), this.Increment * 2, base.Height / 2, this.Increment, base.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.color7), this.Increment * 3, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color8), this.Increment * 3, base.Height / 2, this.Increment, base.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.color9), this.Increment * 4, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color10), this.Increment * 4, base.Height / 2, this.Increment, base.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.color11), this.Increment * 5, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color12), this.Increment * 5, base.Height / 2, this.Increment, base.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.color13), this.Increment * 6, 0, this.Increment, base.Height / 2);
            e.Graphics.FillRectangle(new SolidBrush(this.color14), this.Increment * 6, base.Height / 2, this.Increment, base.Height);
            if (this.showGrid)
            {
                e.Graphics.DrawRectangle(new Pen(this.gridColor, 1f), 0, 0, (this.Increment * 7) - 1, base.Height - 1);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), this.Increment, 0, this.Increment, base.Height);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), this.Increment * 2, 0, this.Increment * 2, base.Height);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), this.Increment * 3, 0, this.Increment * 3, base.Height);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), this.Increment * 4, 0, this.Increment * 4, base.Height);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), this.Increment * 5, 0, this.Increment * 5, base.Height);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), this.Increment * 6, 0, this.Increment * 6, base.Height);
                e.Graphics.DrawLine(new Pen(this.gridColor, 1f), 0, base.Height / 2, (this.Increment * 7) - 1, base.Height / 2);
            }
        }

        // Properties
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color BackColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color ForeColor { get; set; }

        [Category("XanderUI"), Browsable(true), Description("The selected color")]
        public Color SelectedColor
        {
            get =>
                this.selectedColor;
            set
            {
                this.selectedColor = value;
                this.OnColorChange();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the grid")]
        public Color GridColor
        {
            get =>
                this.gridColor;
            set
            {
                this.gridColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show gridlines")]
        public bool ShowGrid
        {
            get =>
                this.showGrid;
            set
            {
                this.showGrid = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 1")]
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

        [Category("XanderUI"), Browsable(true), Description("Color 2")]
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

        [Category("XanderUI"), Browsable(true), Description("Color 3")]
        public Color Color3
        {
            get =>
                this.color3;
            set
            {
                this.color3 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 4")]
        public Color Color4
        {
            get =>
                this.color4;
            set
            {
                this.color4 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 5")]
        public Color Color5
        {
            get =>
                this.color5;
            set
            {
                this.color5 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 6")]
        public Color Color6
        {
            get =>
                this.color6;
            set
            {
                this.color6 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 7")]
        public Color Color7
        {
            get =>
                this.color7;
            set
            {
                this.color7 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 8")]
        public Color Color8
        {
            get =>
                this.color8;
            set
            {
                this.color8 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 9")]
        public Color Color9
        {
            get =>
                this.color9;
            set
            {
                this.color9 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 10")]
        public Color Color10
        {
            get =>
                this.color10;
            set
            {
                this.color10 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 11")]
        public Color Color11
        {
            get =>
                this.color11;
            set
            {
                this.color11 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 12")]
        public Color Color12
        {
            get =>
                this.color12;
            set
            {
                this.color12 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 13")]
        public Color Color13
        {
            get =>
                this.color13;
            set
            {
                this.color13 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Color 14")]
        public Color Color14
        {
            get =>
                this.color14;
            set
            {
                this.color14 = value;
                base.Invalidate();
            }
        }
    }


}
