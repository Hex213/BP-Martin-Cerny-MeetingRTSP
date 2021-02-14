using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUISuperButton : Control
    {
        // Fields
        private Color CurrentBackColor;
        private Color CurrentForeColor;
        private Region NormalRegion;
        private Style buttonStyle = Style.RoundedEdges;
        private Color foreColor = Color.White;
        private Color backColor = Color.FromArgb(0x18, 0xca, 0x8e);
        private bool superSelected;
        private Color hoverForeColor = Color.White;
        private Color hoverBackgroundColor = Color.FromArgb(0x66, 0xd9, 0xae);
        private Color selectedForecolor = Color.White;
        private Color selectedBackcolor = Color.LimeGreen;
        private int cornerRadius = 5;
        private string buttonText = "SuperButton";
        private StringAlignment horizontalAlignment = StringAlignment.Center;
        private StringAlignment verticlAlignment = StringAlignment.Center;
        private Image buttonImage;
        private SmoothingMode buttonSmoothing = SmoothingMode.HighSpeed;
        public imgPosition imagePosition;

        // Methods
        public XUISuperButton()
        {
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            this.CurrentBackColor = this.backColor;
            this.CurrentForeColor = this.foreColor;
            base.Size = new Size(100, 40);
            this.NormalRegion = base.Region;
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.ButtonImage = new Bitmap(base.Height - 2, base.Height - 2);
            Graphics graphics1 = Graphics.FromImage(this.ButtonImage);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.DrawArc(new Pen(Color.White, 2f), new Rectangle(1, 1, this.buttonImage.Width - 3, this.buttonImage.Height - 3), 0f, 360f);
            graphics1.DrawLine(new Pen(Color.White, 2f), (int)(this.buttonImage.Width / 3), (int)(this.buttonImage.Height / 4), (int)((this.buttonImage.Width / 3) * 2), (int)(this.buttonImage.Height / 2));
            graphics1.DrawLine(new Pen(Color.White, 2f), (int)(this.buttonImage.Width / 3), (int)((this.buttonImage.Height / 4) * 3), (int)((this.buttonImage.Width / 3) * 2), (int)(this.buttonImage.Height / 2));
        }

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.CurrentForeColor = this.selectedForecolor;
            this.CurrentBackColor = this.selectedBackcolor;
            this.SuperSelected = true;
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
            if (!this.superSelected)
            {
                this.CurrentForeColor = this.foreColor;
                this.CurrentBackColor = this.backColor;
            }
            else
            {
                this.CurrentForeColor = this.selectedForecolor;
                this.CurrentBackColor = this.selectedBackcolor;
            }
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.superSelected)
            {
                foreach (Control control in base.Parent.Controls)
                {
                    if ((control is XUISuperButton) && (control.Name != base.Name))
                    {
                        ((XUISuperButton)control).SuperSelected = false;
                    }
                }
            }
            e.Graphics.SmoothingMode = this.buttonSmoothing;
            e.Graphics.FillRectangle(new SolidBrush(this.CurrentBackColor), 0, 0, base.Width, base.Height);
            Rectangle layoutRectangle = new Rectangle(0, 0, base.Width, base.Height);
            if (this.buttonImage != null)
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
            }
            StringFormat format = new StringFormat
            {
                LineAlignment = this.verticlAlignment,
                Alignment = this.horizontalAlignment
            };
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString(this.buttonText, this.Font, new SolidBrush(Color.White), layoutRectangle, format);
            base.Region = (this.buttonStyle != Style.Elliptical) ? ((this.buttonStyle != Style.RoundedEdges) ? this.NormalRegion : Region.FromHrgn(CreateRoundRectRgn(0, 0, base.Width, base.Height, this.cornerRadius, this.cornerRadius))) : Region.FromHrgn(CreateRoundRectRgn(0, 0, base.Width, base.Height, base.Width, base.Height));
            base.OnPaint(e);
        }

        // Properties
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color BackColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Color ForeColor { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string Text { get; set; }

        [Category("XanderUI"), Browsable(true), Description("The button style")]
        public Style ButtonStyle
        {
            get =>
                this.buttonStyle;
            set
            {
                this.buttonStyle = value;
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

        [Category("XanderUI"), Browsable(true), Description("Is the SuperButton selected")]
        public bool SuperSelected
        {
            get =>
                this.superSelected;
            set
            {
                this.superSelected = value;
                if (!this.superSelected)
                {
                    this.CurrentForeColor = this.foreColor;
                    this.CurrentBackColor = this.backColor;
                }
                else
                {
                    this.CurrentForeColor = this.selectedForecolor;
                    this.CurrentBackColor = this.selectedBackcolor;
                }
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

        [Category("XanderUI"), Browsable(true), Description("The text color of the button when selected")]
        public Color SelectedTextColor
        {
            get =>
                this.selectedForecolor;
            set
            {
                this.selectedForecolor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The background color of the button when selected")]
        public Color SelectedBackColor
        {
            get =>
                this.selectedBackcolor;
            set
            {
                this.selectedBackcolor = value;
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

        [Category("XanderUI"), Browsable(true), Description("The smoothing mode of the graphics")]
        public SmoothingMode ButtonSmoothing
        {
            get =>
                this.buttonSmoothing;
            set
            {
                this.buttonSmoothing = value;
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
            Right
        }

        public enum Style
        {
            Flat,
            Elliptical,
            RoundedEdges
        }
    }


}
