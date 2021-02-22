using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using LibUIAcademyFramework.Properties;

namespace LibUIAcademy.XanderUI
{
    public class XUIButton : Control
    {
        // Fields
        private Color CurrentBackColor;
        private Color CurrentForeColor;
        private Style buttonStyle = Style.MaterialRounded;
        private Color foreColor = Color.DodgerBlue;
        private Color backColor = Color.FromArgb(0xff, 0xff, 0xff);
        private Color hoverForeColor = Color.DodgerBlue;
        private Color hoverBackgroundColor = Color.FromArgb(0xe1, 0xe1, 0xe1);
        private Color clickForecolor = Color.DodgerBlue;
        private Color clickBackcolor = Color.FromArgb(0xc3, 0xc3, 0xc3);
        private int cornerRadius = 5;
        private string buttonText = "Button";
        private StringAlignment horizontalAlignment = StringAlignment.Center;
        private StringAlignment verticlAlignment = StringAlignment.Center;
        private Image buttonImage = Resources.XUIMini;
        public imgPosition imagePosition;

        // Methods
        public XUIButton()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
            base.Size = new Size(200, 50);
            this.CurrentBackColor = this.backColor;
            this.CurrentForeColor = this.foreColor;
            this.BackColor = Color.Transparent;
        }

        private void DrawRoundedRectangle(Graphics Gfx, Color borderColor, int CornerRadius, int borderThickness)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc((base.Width - (CornerRadius * 2)) - 2, 0, CornerRadius * 2, CornerRadius * 2, 270f, 90f);
                path.AddArc((int)((base.Width - (CornerRadius * 2)) - 2), (int)(base.Height - (CornerRadius * 2)), (int)(CornerRadius * 2), (int)((CornerRadius * 2) - 2), 0f, 90f);
                path.AddArc(0, (base.Height - (CornerRadius * 2)) - 2, (CornerRadius * 2) - 2, CornerRadius * 2, 90f, 90f);
                path.AddArc(0, 0, CornerRadius * 2, CornerRadius * 2, 180f, 90f);
                path.CloseFigure();
                Gfx.DrawPath(new Pen(borderColor, (float)borderThickness), path);
            }
        }

        private void FillRoundedRectangle(Graphics Gfx, Color ButtonColor, int CornerRadius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc((base.Width - (CornerRadius * 2)) - 2, 0, CornerRadius * 2, CornerRadius * 2, 270f, 90f);
                path.AddArc((int)((base.Width - (CornerRadius * 2)) - 2), (int)(base.Height - (CornerRadius * 2)), (int)(CornerRadius * 2), (int)((CornerRadius * 2) - 2), 0f, 90f);
                path.AddArc(0, (base.Height - (CornerRadius * 2)) - 2, (CornerRadius * 2) - 2, CornerRadius * 2, 90f, 90f);
                path.AddArc(0, 0, CornerRadius * 2, CornerRadius * 2, 180f, 90f);
                path.CloseFigure();
                Gfx.FillPath(new SolidBrush(ButtonColor), path);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.CurrentForeColor = this.clickForecolor;
            this.CurrentBackColor = this.clickBackcolor;
            base.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.CurrentForeColor = this.hoverForeColor;
            this.CurrentBackColor = this.hoverBackgroundColor;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.CurrentForeColor = this.foreColor;
            this.CurrentBackColor = this.backColor;
            base.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.CurrentForeColor = this.foreColor;
            this.CurrentBackColor = this.backColor;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (this.buttonStyle == Style.MaterialEllipse)
            {
                e.Graphics.FillPie(new SolidBrush(this.CurrentBackColor), 0, 0, base.Width - 1, base.Height - 1, 0, 360);
            }
            if (this.buttonStyle == Style.Material)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.CurrentBackColor), 0, 0, base.Width, base.Height);
            }
            if (this.buttonStyle == Style.MaterialRounded)
            {
                if (((base.Height / 2) - 1) != 0)
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, this.cornerRadius);
                }
                else
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, this.cornerRadius);
                }
            }
            if (this.buttonStyle == Style.Invert)
            {
                if (((base.Height / 2) - 1) != 0)
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, (base.Height / 2) - 1);
                }
                else
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, base.Height / 2);
                }
                if (((base.Height / 2) - 1) != 0)
                {
                    this.DrawRoundedRectangle(e.Graphics, this.CurrentForeColor, (base.Height / 2) - 1, 2);
                }
                else
                {
                    this.DrawRoundedRectangle(e.Graphics, this.CurrentForeColor, base.Height / 2, 2);
                }
                this.hoverBackgroundColor = this.foreColor;
                this.hoverForeColor = this.backColor;
                this.clickBackcolor = this.foreColor;
                this.clickForecolor = this.foreColor;
            }
            if (this.buttonStyle == Style.Dark)
            {
                this.backColor = Color.FromArgb(0x41, 70, 0x4b);
                this.foreColor = Color.FromArgb(0xc3, 200, 0xb9);
                this.hoverBackgroundColor = Color.FromArgb(0x4b, 80, 90);
                this.hoverForeColor = Color.FromArgb(0xeb, 0xeb, 0xd7);
                this.clickBackcolor = Color.FromArgb(0x41, 0x4b, 80);
                this.clickForecolor = Color.FromArgb(0x7d, 130, 140);
                if (((base.Height / 2) - 1) != 0)
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, (base.Height / 2) - 1);
                }
                else
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, base.Height / 2);
                }
                if (((base.Height / 2) - 1) != 0)
                {
                    this.DrawRoundedRectangle(e.Graphics, Color.Black, (base.Height / 2) - 1, 1);
                }
                else
                {
                    this.DrawRoundedRectangle(e.Graphics, Color.Black, base.Height / 2, 1);
                }
            }
            if (this.buttonStyle == Style.MacOS)
            {
                this.backColor = Color.White;
                this.foreColor = Color.Black;
                this.Font = new Font("Microsoft Sans Serif", 14f);
                if (((base.Height / 2) - 1) != 0)
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, 8);
                }
                else
                {
                    this.FillRoundedRectangle(e.Graphics, this.CurrentBackColor, 8);
                }
                if (((base.Height / 2) - 1) != 0)
                {
                    this.DrawRoundedRectangle(e.Graphics, Color.FromArgb(0xa3, 0xa3, 0xa3), 8, 2);
                }
                else
                {
                    this.DrawRoundedRectangle(e.Graphics, Color.FromArgb(0xa3, 0xa3, 0xa3), 8, 2);
                }
            }
            Rectangle layoutRectangle = new Rectangle(0, 0, base.Width, base.Height);
            if ((this.buttonStyle != Style.Dark) && ((this.buttonStyle != Style.MaterialEllipse) && ((this.buttonStyle != Style.MacOS) && ((this.buttonStyle != Style.Invert) && (this.buttonImage != null)))))
            {
                if (this.imagePosition == imgPosition.Left)
                {
                    layoutRectangle = new Rectangle(base.Height, 0, base.Width - base.Height, base.Height);
                    e.Graphics.DrawImage(new Bitmap(this.buttonImage, base.Height - 2, base.Height - 2), 1, 1);
                }
                if (this.imagePosition == imgPosition.Right)
                {
                    layoutRectangle = new Rectangle(0, 0, base.Width - base.Height, base.Height);
                    e.Graphics.DrawImage(new Bitmap(this.buttonImage, base.Height - 2, base.Height - 2), base.Width - base.Height, 1);
                }
                if (this.imagePosition == imgPosition.Center)
                {
                    e.Graphics.DrawImage(new Bitmap(this.buttonImage, base.Height - 2, base.Height - 2), (base.Width / 2) - (base.Height / 2), 1);
                }
            }
            if ((this.imagePosition != imgPosition.Center) || (this.buttonImage == null))
            {
                StringFormat format = new StringFormat
                {
                    LineAlignment = this.verticlAlignment,
                    Alignment = this.horizontalAlignment
                };
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(this.buttonText, this.Font, new SolidBrush(this.CurrentForeColor), layoutRectangle, format);
            }
        }

        // Properties
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text { get; set; }

        [Category("XanderUI"), Browsable(true), Description("The button style")]
        public Style ButtonStyle
        {
            get =>
                this.buttonStyle;
            set
            {
                this.buttonStyle = value;
                if (this.buttonStyle == Style.Dark)
                {
                    this.CurrentBackColor = Color.FromArgb(0x41, 70, 0x4b);
                    this.CurrentForeColor = Color.FromArgb(0xc3, 200, 0xb9);
                }
                if (this.buttonStyle == Style.MacOS)
                {
                    this.CurrentBackColor = Color.White;
                    this.CurrentForeColor = Color.Black;
                }
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text color of the button")]
        public Color TextColor
        {
            get =>
                this.foreColor;
            set
            {
                this.foreColor = value;
                this.CurrentForeColor = this.foreColor;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The background color of the button")]
        public Color BackgroundColor
        {
            get =>
                this.backColor;
            set
            {
                this.backColor = value;
                this.CurrentBackColor = this.backColor;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text color of the button while the mouse is over it")]
        public Color HoverTextColor
        {
            get =>
                this.hoverForeColor;
            set
            {
                this.hoverForeColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The background color of the button while the mouse is over it")]
        public Color HoverBackgroundColor
        {
            get =>
                this.hoverBackgroundColor;
            set
            {
                this.hoverBackgroundColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text color of the button when clicked")]
        public Color ClickTextColor
        {
            get =>
                this.clickForecolor;
            set
            {
                this.clickForecolor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The background color of the button when clicked")]
        public Color ClickBackColor
        {
            get =>
                this.clickBackcolor;
            set
            {
                this.clickBackcolor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The corner radius if rounded edges")]
        public int CornerRadius
        {
            get =>
                this.cornerRadius;
            set
            {
                this.cornerRadius = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text of the button")]
        public string ButtonText
        {
            get =>
                this.buttonText;
            set
            {
                this.buttonText = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text horizontal alignment")]
        public StringAlignment Horizontal_Alignment
        {
            get =>
                this.horizontalAlignment;
            set
            {
                this.horizontalAlignment = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text vertical alignment")]
        public StringAlignment Vertical_Alignment
        {
            get =>
                this.verticlAlignment;
            set
            {
                this.verticlAlignment = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The buttons image")]
        public Image ButtonImage
        {
            get =>
                this.buttonImage;
            set
            {
                this.buttonImage = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Button image position")]
        public imgPosition ImagePosition
        {
            get =>
                this.imagePosition;
            set
            {
                this.imagePosition = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum imgPosition
        {
            Left,
            Right,
            Center
        }

        public enum Style
        {
            Material,
            Dark,
            MacOS,
            Invert,
            MaterialRounded,
            MaterialEllipse
        }
    }


}
