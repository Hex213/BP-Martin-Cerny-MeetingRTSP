using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUILineGraph : Control
    {
        // Fields
        private List<int> items = new List<int>();
        private bool showVerticalLines;
        private bool showBorder;
        private bool showTitle;
        private bool showPoints = true;
        private StringAlignment titleAlignment;
        private int pointSize = 7;
        private Color backgroundColor = Color.FromArgb(0x66, 0xd9, 0xae);
        private Color belowLineColor = Color.FromArgb(0x18, 0xca, 0x8e);
        private Color borderColor = Color.White;
        private Color lineColor = Color.White;
        private Color verticalLineColor = Color.DimGray;
        private Color graphTitleColor = Color.Gray;
        private string graphTitle = "XUI LineGraph";
        private Style graphStyle = Style.Material;

        // Methods
        public XUILineGraph()
        {
            this.DoubleBuffered = true;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.Size = new Size(200, 100);
            this.items.Add(50);
            this.items.Add(20);
            this.items.Add(100);
            this.items.Add(60);
            this.items.Add(1);
            this.items.Add(20);
            this.items.Add(80);
            this.items.Add(12);
            this.items.Add(0x48);
            this.items.Add(0x3a);
            this.items.Add(0x13);
            this.items.Add(600);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Pen pen = new Pen(this.lineColor, 1f);
            Pen pen2 = new Pen(this.verticalLineColor, 1f);
            if (this.graphStyle == Style.Material)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(40, 40, 40)), new Rectangle(0, 0, base.Width, base.Height));
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(this.backgroundColor), new Rectangle(0, 0, base.Width, base.Height));
            }
            int total = this.items.ToArray().Max();
            int num2 = base.Width / this.items.Count;
            int num3 = 0;
            int height = base.Height;
            int num5 = num2;
            int num6 = 0;
            List<PointF> list = new List<PointF> {
            (PointF) new Point(1, base.Height)
        };
            foreach (int num7 in this.items)
            {
                if (num7 > 0)
                {
                    int num8 = XUIPercentage.IntToPercent(num7, total);
                    num6 = (num8 <= 0x61) ? ((num8 >= 3) ? (base.Height - ((num8 * base.Height) / 100)) : (base.Height - XUIPercentage.PercentToInt(3, base.Height))) : (base.Height - XUIPercentage.PercentToInt(0x61, base.Height));
                    list.Add((PointF)new Point(num5 - 1, num6 - 1));
                    num3 = num5;
                    height = num6;
                    num5 += num2;
                }
            }
            list.Add((PointF)new Point(base.Width, num6 - 1));
            if (this.graphStyle == Style.Curved)
            {
                if (this.showPoints)
                {
                    foreach (PointF tf in list)
                    {
                        if (((tf.Y - (this.pointSize / 2)) - 1f) < 0f)
                        {
                            e.Graphics.FillEllipse(new SolidBrush(this.lineColor), new RectangleF((tf.X - (this.pointSize / 2)) - 1f, -1f, (float)this.pointSize, (float)this.pointSize));
                            continue;
                        }
                        if ((((tf.Y - (this.pointSize / 2)) - 1f) + this.pointSize) > base.Height)
                        {
                            e.Graphics.FillEllipse(new SolidBrush(this.lineColor), new RectangleF((tf.X - (this.pointSize / 2)) - 1f, (float)((base.Height - this.pointSize) + 1), (float)this.pointSize, (float)this.pointSize));
                            continue;
                        }
                        e.Graphics.FillEllipse(new SolidBrush(this.lineColor), new RectangleF((tf.X - (this.pointSize / 2)) - 1f, (tf.Y - (this.pointSize / 2)) - 1f, (float)this.pointSize, (float)this.pointSize));
                    }
                }
                e.Graphics.DrawCurve(pen, list.ToArray());
            }
            else
            {
                list.Add((PointF)new Point(base.Width, base.Height));
                if (this.graphStyle == Style.Flat)
                {
                    SolidBrush brush = new SolidBrush(this.belowLineColor);
                    e.Graphics.FillPolygon(brush, list.ToArray());
                }
                else
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height), Color.FromArgb(0xf9, 0x37, 0x62), Color.FromArgb(0, 0xa2, 250), 1f);
                    e.Graphics.FillPolygon(brush, list.ToArray());
                }
                num3 = 1;
                height = base.Height;
                num5 = num2;
                num6 = 0;
                int num9 = 0;
                using (List<int>.Enumerator enumerator = this.items.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        int num10 = XUIPercentage.IntToPercent(enumerator.Current, total);
                        num6 = (num10 <= 0x61) ? ((num10 >= 3) ? (base.Height - ((num10 * base.Height) / 100)) : (base.Height - XUIPercentage.PercentToInt(3, base.Height))) : (base.Height - XUIPercentage.PercentToInt(0x61, base.Height));
                        if (((this.graphStyle == Style.Flat) && this.showVerticalLines) && (((num9 + 1) != this.items.ToArray().Length) && ((num5 != 0) && (num5 != base.Width))))
                        {
                            e.Graphics.DrawLine(pen2, num5, base.Height, num5, 0);
                        }
                        e.Graphics.DrawLine(pen, (int)(num3 - 1), (int)(height - 1), (int)(num5 - 1), (int)(num6 - 1));
                        if (this.showPoints)
                        {
                            if (((num6 - (this.pointSize / 2)) - 1) < 0)
                            {
                                e.Graphics.FillEllipse(new SolidBrush(this.lineColor), new RectangleF((float)((num5 - (this.pointSize / 2)) - 1), -1f, (float)this.pointSize, (float)this.pointSize));
                            }
                            else if ((((num6 - (this.pointSize / 2)) - 1) + this.pointSize) > base.Height)
                            {
                                e.Graphics.FillEllipse(new SolidBrush(this.lineColor), new RectangleF((float)((num5 - (this.pointSize / 2)) - 1), (float)((base.Height - this.pointSize) + 1), (float)this.pointSize, (float)this.pointSize));
                            }
                            else
                            {
                                e.Graphics.FillEllipse(new SolidBrush(this.lineColor), new RectangleF((float)((num5 - (this.pointSize / 2)) - 1), (float)((num6 - (this.pointSize / 2)) - 1), (float)this.pointSize, (float)this.pointSize));
                            }
                        }
                        num3 = num5;
                        height = num6;
                        num5 += num2;
                    }
                }
                e.Graphics.DrawLine(pen, num3, height, base.Width, height);
            }
            if ((this.graphStyle != Style.Material) && this.showBorder)
            {
                e.Graphics.DrawRectangle(new Pen(this.borderColor, 2f), new Rectangle(0, 0, base.Width - 1, base.Height - 1));
            }
            if (this.showTitle)
            {
                StringFormat format = new StringFormat
                {
                    LineAlignment = StringAlignment.Near,
                    Alignment = this.titleAlignment
                };
                Font font = new Font("Arial", 14f);
                RectangleF layoutRectangle = new RectangleF(0f, 0f, (float)base.Width, (float)base.Height);
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(this.graphTitle, font, new SolidBrush(this.graphTitleColor), layoutRectangle, format);
            }
            base.OnPaint(e);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public List<int> Items
        {
            get =>
                this.items;
            set
            {
                this.items = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public bool ShowVerticalLines
        {
            get =>
                this.showVerticalLines;
            set
            {
                this.showVerticalLines = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public Color BackGroundColor
        {
            get =>
                this.backgroundColor;
            set
            {
                this.backgroundColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public Color BelowLineColor
        {
            get =>
                this.belowLineColor;
            set
            {
                this.belowLineColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public Color LineColor
        {
            get =>
                this.lineColor;
            set
            {
                this.lineColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
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

        [Category("XanderUI"), Browsable(true), Description("The color of the text when the tab is selected")]
        public Color VerticalLineColor
        {
            get =>
                this.verticalLineColor;
            set
            {
                this.verticalLineColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the graph title")]
        public Color GraphTitleColor
        {
            get =>
                this.graphTitleColor;
            set
            {
                this.graphTitleColor = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The of the graph")]
        public string GraphTitle
        {
            get =>
                this.graphTitle;
            set
            {
                this.graphTitle = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Draw the title on the control")]
        public bool ShowTitle
        {
            get =>
                this.showTitle;
            set
            {
                this.showTitle = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Draw the border on the control")]
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

        [Category("XanderUI"), Browsable(true), Description("Draw the points on each value")]
        public bool ShowPoints
        {
            get =>
                this.showPoints;
            set
            {
                this.showPoints = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The point size")]
        public int PointSize
        {
            get =>
                this.pointSize;
            set
            {
                this.pointSize = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The title alignment")]
        public StringAlignment TitleAlignment
        {
            get =>
                this.titleAlignment;
            set
            {
                this.titleAlignment = value;
                this.Refresh();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The style of the graph")]
        public Style GraphStyle
        {
            get =>
                this.graphStyle;
            set
            {
                this.graphStyle = value;
                this.Refresh();
            }
        }

        // Nested Types
        public enum Style
        {
            Flat,
            Material,
            Curved
        }
    }


}
