using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUICard : Control
    {
        // Fields
        private Color color1 = Color.DodgerBlue;
        private Color color2 = Color.LimeGreen;
        private string text1 = "Savings Card";
        private string text2 = "1234 5678 9101 1121";
        private string text3 = "Exp: 01/02 - 03/04";

        // Methods
        public XUICard()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
            base.Size = new Size(320, 170);
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (base.Width > base.Height)
            {
                int width = base.Width;
            }
            else
            {
                int height = base.Height;
            }
            Brush brush = new LinearGradientBrush(base.ClientRectangle, this.color1, this.color2, 0x87f);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc((base.Width - 10) - 2, 0, 10, 10, 250f, 90f);
                path.AddArc((base.Width - 10) - 2, base.Height - 10, 10, 8, 0f, 90f);
                path.AddArc(0, (base.Height - 10) - 2, 8, 10, 90f, 90f);
                path.AddArc(0, 0, 10, 10, 180f, 90f);
                path.CloseFigure();
                e.Graphics.FillPath(brush, path);
            }
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Rectangle layoutRectangle = new Rectangle(2, 6, base.Width - 4, 0x1a);
            e.Graphics.DrawString(this.text1, new Font(this.Font.FontFamily, this.Font.Size + 4f), new SolidBrush(this.ForeColor), layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new Rectangle(2, base.Height / 2, base.Width - 4, base.Height / 4);
            e.Graphics.DrawString(this.text2, new Font(this.Font.FontFamily, (this.Font.Size * 2f) + 2f), new SolidBrush(this.ForeColor), layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new Rectangle(2, (base.Height / 2) + (base.Height / 4), base.Width - 4, base.Height / 4);
            e.Graphics.DrawString(this.text3, new Font(this.Font.FontFamily, this.Font.Size + 2f), new SolidBrush(this.ForeColor), layoutRectangle, format);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The 1st half color of he gradient")]
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

        [Category("XanderUI"), Browsable(true), Description("The 2nd half color of he gradient")]
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

        [Category("XanderUI"), Browsable(true), Description("The 1st text")]
        public string Text1
        {
            get =>
                this.text1;
            set
            {
                this.text1 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The 2nd text")]
        public string Text2
        {
            get =>
                this.text2;
            set
            {
                this.text2 = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The 3rd text")]
        public string Text3
        {
            get =>
                this.text3;
            set
            {
                this.text3 = value;
                base.Invalidate();
            }
        }
    }


}
