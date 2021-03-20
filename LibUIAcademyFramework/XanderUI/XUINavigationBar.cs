using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUINavigationBar : Control
    {
        // Fields
        private Style navBarStyle = Style.Android;
        private Color itemColor = Color.White;
        private Color titleColor = Color.White;
        private Color backgroundColor = Color.FromArgb(1, 0x77, 0xd7);
        private NavigationItem leftItem = NavigationItem.Back;
        private NavigationItem rightItem = NavigationItem.Next;
        private string title = "Navigation Bar";
        private string leftCustomText = "CustomBack";
        private string rightCustomText = "CustomNext";
        private Image leftCustomImage;
        private Image rightCustomImage;

        // Events
        public event EventHandler LeftItemClick;

        public event EventHandler RightItemClick;

        // Methods
        public XUINavigationBar()
        {
            base.Size = new Size(300, 40);
        }

        protected virtual void OnLeftItemClick()
        {
            if (this.LeftItemClick != null)
            {
                this.LeftItemClick(this, new EventArgs());
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.X < (base.Width / 3))
            {
                this.OnLeftItemClick();
            }
            if (e.X > ((base.Width / 3) * 2))
            {
                this.OnRightItemClick();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Color white = Color.White;
            Color color = Color.White;
            Color backgroundColor = Color.White;
            FontStyle bold = FontStyle.Bold;
            if (this.navBarStyle == Style.iOS)
            {
                white = Color.FromArgb(0, 120, 0xff);
                color = Color.Black;
                backgroundColor = Color.White;
                bold = FontStyle.Regular;
            }
            if (this.navBarStyle == Style.Android)
            {
                white = Color.White;
                color = Color.White;
                backgroundColor = Color.FromArgb(0, 150, 0x87);
            }
            if (this.navBarStyle == Style.Material)
            {
                white = this.itemColor;
                color = this.titleColor;
                backgroundColor = this.backgroundColor;
            }
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), 0, 0, base.Width, base.Height);
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };
            if (this.leftItem == NavigationItem.Back)
            {
                e.Graphics.DrawString("Back", new Font("Arial", 12f), new SolidBrush(white), base.ClientRectangle, format);
            }
            else if (this.leftItem == NavigationItem.Next)
            {
                e.Graphics.DrawString("Next", new Font("Arial", 12f), new SolidBrush(white), base.ClientRectangle, format);
            }
            else if (this.leftItem == NavigationItem.CustomText)
            {
                e.Graphics.DrawString(this.leftCustomText, new Font("Arial", 12f), new SolidBrush(white), base.ClientRectangle, format);
            }
            else if (this.leftItem == NavigationItem.Menu)
            {
                e.Graphics.DrawLine(new Pen(white, 2f), (int)(base.Height / 5), (int)(base.Height / 4), (int)((base.Height / 5) * 4), (int)(base.Height / 4));
                e.Graphics.DrawLine(new Pen(white, 2f), (int)(base.Height / 5), (int)((base.Height / 4) * 2), (int)((base.Height / 5) * 4), (int)((base.Height / 4) * 2));
                e.Graphics.DrawLine(new Pen(white, 2f), (int)(base.Height / 5), (int)((base.Height / 4) * 3), (int)((base.Height / 5) * 4), (int)((base.Height / 4) * 3));
            }
            else if ((this.leftItem == NavigationItem.CustomImage) && (this.leftCustomImage != null))
            {
                e.Graphics.DrawImage(new Bitmap(this.leftCustomImage, base.Height, base.Height), 0, 0);
            }
            format.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(this.title, new Font("Arial", 12f, bold), new SolidBrush(color), base.ClientRectangle, format);
            format.Alignment = StringAlignment.Far;
            if (this.rightItem == NavigationItem.Back)
            {
                e.Graphics.DrawString("Back", new Font("Arial", 12f), new SolidBrush(white), base.ClientRectangle, format);
            }
            else if (this.rightItem == NavigationItem.Next)
            {
                e.Graphics.DrawString("Next", new Font("Arial", 12f), new SolidBrush(white), base.ClientRectangle, format);
            }
            else if (this.rightItem == NavigationItem.CustomText)
            {
                e.Graphics.DrawString(this.rightCustomText, new Font("Arial", 12f), new SolidBrush(white), base.ClientRectangle, format);
            }
            else if (this.rightItem == NavigationItem.Menu)
            {
                e.Graphics.DrawLine(new Pen(white, 2f), (int)((base.Width - base.Height) + (base.Height / 5)), (int)(base.Height / 4), (int)((base.Width - base.Height) + ((base.Height / 5) * 4)), (int)(base.Height / 4));
                e.Graphics.DrawLine(new Pen(white, 2f), (int)((base.Width - base.Height) + (base.Height / 5)), (int)((base.Height / 4) * 2), (int)((base.Width - base.Height) + ((base.Height / 5) * 4)), (int)((base.Height / 4) * 2));
                e.Graphics.DrawLine(new Pen(white, 2f), (int)((base.Width - base.Height) + (base.Height / 5)), (int)((base.Height / 4) * 3), (int)((base.Width - base.Height) + ((base.Height / 5) * 4)), (int)((base.Height / 4) * 3));
            }
            else if ((this.rightItem == NavigationItem.CustomImage) && (this.rightCustomImage != null))
            {
                e.Graphics.DrawImage(new Bitmap(this.rightCustomImage, base.Height, base.Height), base.Width - base.Height, 0);
            }
        }

        protected virtual void OnRightItemClick()
        {
            if (this.RightItemClick != null)
            {
                this.RightItemClick(this, new EventArgs());
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The navigation bar style")]
        public Style NavBarStyle
        {
            get =>
                this.navBarStyle;
            set
            {
                this.navBarStyle = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the items")]
        public Color ItemColor
        {
            get =>
                this.itemColor;
            set
            {
                this.itemColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the title")]
        public Color TitleColor
        {
            get =>
                this.titleColor;
            set
            {
                this.titleColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the title")]
        public Color BackgroundColor
        {
            get =>
                this.backgroundColor;
            set
            {
                this.backgroundColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The left navigation item")]
        public NavigationItem LeftItem
        {
            get =>
                this.leftItem;
            set
            {
                this.leftItem = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The right navigation item")]
        public NavigationItem RightItem
        {
            get =>
                this.rightItem;
            set
            {
                this.rightItem = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The navigation bar title")]
        public string Title
        {
            get =>
                this.title;
            set
            {
                this.title = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text of the left item if set to CustomText")]
        public string LeftCustomText
        {
            get =>
                this.leftCustomText;
            set
            {
                this.leftCustomText = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text of the right item if set to CustomText")]
        public string RightCustomText
        {
            get =>
                this.rightCustomText;
            set
            {
                this.rightCustomText = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The image of the left item if set to CustomImage")]
        public Image LeftCustomImage
        {
            get =>
                this.leftCustomImage;
            set
            {
                this.leftCustomImage = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The image of the right item if set to CustomImage")]
        public Image RightCustomImage
        {
            get =>
                this.rightCustomImage;
            set
            {
                this.rightCustomImage = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum NavigationItem
        {
            Menu,
            None,
            Back,
            Next,
            CustomText,
            CustomImage
        }

        public enum Style
        {
            iOS,
            Android,
            Material
        }
    }


}
