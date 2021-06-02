using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIFlatTab : TabControl
    {
        // Fields
        private Color activeColor = Color.DodgerBlue;
        private Color inActiveColor = Color.RoyalBlue;
        private Color backTabColor = Color.White;
        private Color borderColor = Color.DodgerBlue;
        private Color headerBackgroundColor = Color.White;
        private Color activeTextColor = Color.White;
        private Color inActiveTextColor = Color.White;
        private bool showBorder = true;

        // Methods
        public XUIFlatTab()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            base.SizeMode = TabSizeMode.Normal;
            base.ItemSize = new Size(240, 0x10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.FillRectangle(new SolidBrush(this.headerBackgroundColor), new Rectangle(0, 0, base.Width, base.Height));
            try
            {
                base.SelectedTab.BackColor = this.backTabColor;
            }
            catch
            {
            }
            try
            {
                base.SelectedTab.BorderStyle = BorderStyle.None;
            }
            catch
            {
            }
            for (int i = 0; i <= (base.TabCount - 1); i++)
            {
                Point location = base.GetTabRect(i).Location;
                Rectangle tabRect = base.GetTabRect(i);
                Rectangle rectangle = new Rectangle(new Point(base.GetTabRect(i).Location.X + 2, location.Y), new Size(base.GetTabRect(i).Width, tabRect.Height));
                Rectangle rect = new Rectangle(rectangle.Location, new Size(rectangle.Width, rectangle.Height));
                if (i != base.SelectedIndex)
                {
                    graphics.FillRectangle(new SolidBrush(this.inActiveColor), new Rectangle(rectangle.X - 4, rectangle.Y + 1, rectangle.Width - 1, rectangle.Height + 5));
                    graphics.DrawString(base.TabPages[i].Text, this.Font, new SolidBrush(this.inActiveTextColor), (float)rect.X, (float)((rect.Y / 2) + 3));
                }
                else
                {
                    graphics.FillRectangle(new SolidBrush(this.headerBackgroundColor), rect);
                    graphics.FillRectangle(new SolidBrush(this.activeColor), new Rectangle(rectangle.X - 4, rectangle.Y - 3, rectangle.Width - 1, rectangle.Height + 5));
                    graphics.DrawString(base.TabPages[i].Text, this.Font, new SolidBrush(this.activeTextColor), (float)rect.X, (float)((rect.Y / 2) + 1));
                }
            }
            graphics.FillRectangle(new SolidBrush(this.backTabColor), new Rectangle(1, 0x13, base.Width - 3, base.Height - 20));
            if (this.showBorder)
            {
                graphics.DrawRectangle(new Pen(this.borderColor, 1f), new Rectangle(1, 20, base.Width - 1, base.Height - 20));
            }
            else
            {
                graphics.DrawLine(new Pen(this.borderColor, 1f), 0, 20, base.Width, 20);
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public Color ActiveTextColor
        {
            get =>
                this.activeTextColor;
            set
            {
                this.activeTextColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the header when the tab is selected")]
        public Color ActiveHeaderColor
        {
            get =>
                this.activeColor;
            set
            {
                this.activeColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is not selected")]
        public Color InActiveTextColor
        {
            get =>
                this.inActiveTextColor;
            set
            {
                this.inActiveTextColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the header when the tab is not selected")]
        public Color InActiveHeaderColor
        {
            get =>
                this.inActiveColor;
            set
            {
                this.inActiveColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the header background")]
        public Color HeaderBackgroundColor
        {
            get =>
                this.headerBackgroundColor;
            set
            {
                this.headerBackgroundColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the border")]
        public Color BorderColor
        {
            get =>
                this.borderColor;
            set
            {
                this.borderColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The background color of the pages")]
        public Color PageColor
        {
            get =>
                this.backTabColor;
            set
            {
                this.backTabColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the border")]
        public bool ShowBorder
        {
            get =>
                this.showBorder;
            set
            {
                this.showBorder = value;
                this.Refresh();
            }
        }
    }



}
