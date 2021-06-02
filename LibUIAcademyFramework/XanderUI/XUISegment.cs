using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUISegment : Control
    {
        // Fields
        private string items = "Contacts, Recents, Messages, Dialer";
        private int selectedIndex;
        private Style segmentStyle = Style.Material;
        private Color segmentColor = Color.White;
        private Color segmentBackColor = Color.FromArgb(0, 150, 0x87);
        private Color segmentActiveTextColor = Color.White;
        private Color segmentInactiveTextColor = Color.FromArgb(150, 210, 210);

        // Events
        public event EventHandler IndexChanged;

        // Methods
        public XUISegment()
        {
            base.Size = new Size(240, 30);
            this.Cursor = Cursors.Hand;
        }

        protected virtual void OnIndexChanged()
        {
            if (this.IndexChanged != null)
            {
                this.IndexChanged(this, new EventArgs());
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int num = 0;
            int num2 = 0;
            char[] separator = new char[] { ',' };
            string[] strArray = this.items.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string text1 = strArray[i];
                num2++;
            }
            int num3 = base.Width / num2;
            if (e.X > 0)
            {
                num = 0;
            }
            if (e.X > num3)
            {
                num = 1;
            }
            if (e.X > (num3 * 2))
            {
                num = 2;
            }
            if (e.X > (num3 * 3))
            {
                num = 3;
            }
            if (e.X > (num3 * 4))
            {
                num = 4;
            }
            if (e.X > (num3 * 5))
            {
                num = 5;
            }
            if (e.X > (num3 * 6))
            {
                num = 6;
            }
            if (e.X > (num3 * 7))
            {
                num = 7;
            }
            if (e.X > (num3 * 8))
            {
                num = 8;
            }
            if (e.X > (num3 * 9))
            {
                num = 9;
            }
            if (e.X > (num3 * 10))
            {
                num = 10;
            }
            if (num != this.selectedIndex)
            {
                this.SelectedIndex = num;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            int num = 0;
            char[] separator = new char[] { ',' };
            string[] strArray = this.items.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string text1 = strArray[i];
                num++;
            }
            int width = base.Width / num;
            int num3 = 0;
            int x = 0;
            if (this.segmentStyle == Style.iOS)
            {
                char[] chArray2 = new char[] { ',' };
                foreach (string str in this.items.Split(chArray2))
                {
                    if (num3 <= num)
                    {
                        Rectangle layoutRectangle = new Rectangle(x, 0, width, base.Height);
                        StringFormat format = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        e.Graphics.DrawRectangle(new Pen(Color.FromArgb(0, 120, 0xff), 1f), 0, 0, base.Width - 1, base.Height - 1);
                        if (this.selectedIndex == num3)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 120, 0xff)), x, 0, width, base.Height);
                            e.Graphics.DrawString(str, this.Font, new SolidBrush(Color.White), layoutRectangle, format);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(0, 120, 0xff), 1f), x, 0, x + width, base.Height - 1);
                            e.Graphics.DrawString(str, this.Font, new SolidBrush(Color.FromArgb(0, 120, 0xff)), layoutRectangle, format);
                        }
                    }
                    x += width;
                    num3++;
                }
            }
            if (this.segmentStyle == Style.Android)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, base.Width, base.Height);
                char[] chArray3 = new char[] { ',' };
                foreach (string str2 in this.items.Split(chArray3))
                {
                    if (num3 <= num)
                    {
                        Rectangle layoutRectangle = new Rectangle(x, 0, width, base.Height - 5);
                        StringFormat format = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        if (this.selectedIndex != num3)
                        {
                            e.Graphics.DrawString(str2, this.Font, new SolidBrush(Color.FromArgb(0x99, 0x99, 0x99)), layoutRectangle, format);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x41, 130, 0xcd)), x, base.Height - 3, width, 3);
                            e.Graphics.DrawString(str2, this.Font, new SolidBrush(Color.FromArgb(0x41, 130, 0xcd)), layoutRectangle, format);
                        }
                    }
                    x += width;
                    num3++;
                }
            }
            if (this.segmentStyle == Style.Material)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.segmentBackColor), 0, 0, base.Width, base.Height);
                char[] chArray4 = new char[] { ',' };
                foreach (string str3 in this.items.Split(chArray4))
                {
                    if (num3 <= num)
                    {
                        Rectangle layoutRectangle = new Rectangle(x, 0, width, base.Height - 5);
                        StringFormat format = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        if (this.selectedIndex != num3)
                        {
                            e.Graphics.DrawString(str3, this.Font, new SolidBrush(this.segmentInactiveTextColor), layoutRectangle, format);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(this.segmentColor), x, base.Height - 3, width, 3);
                            e.Graphics.DrawString(str3, this.Font, new SolidBrush(this.segmentActiveTextColor), layoutRectangle, format);
                        }
                    }
                    x += width;
                    num3++;
                }
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The items, split by ','.")]
        public string Items
        {
            get =>
                this.items;
            set
            {
                this.items = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The selected index")]
        public int SelectedIndex
        {
            get =>
                this.selectedIndex;
            set
            {
                this.selectedIndex = value;
                this.OnIndexChanged();
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The segment style")]
        public Style SegmentStyle
        {
            get =>
                this.segmentStyle;
            set
            {
                this.segmentStyle = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The segment selected base color")]
        public Color SegmentColor
        {
            get =>
                this.segmentColor;
            set
            {
                this.segmentColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The segment back color")]
        public Color SegmentBackColor
        {
            get =>
                this.segmentBackColor;
            set
            {
                this.segmentBackColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The active segment text color")]
        public Color SegmentActiveTextColor
        {
            get =>
                this.segmentActiveTextColor;
            set
            {
                this.segmentActiveTextColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Theinactive segment text color")]
        public Color SegmentInactiveTextColor
        {
            get =>
                this.segmentInactiveTextColor;
            set
            {
                this.segmentInactiveTextColor = value;
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
