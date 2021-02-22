using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIBarGraph : Control
    {
        // Fields
        private List<int> items = new List<int>();
        private Color filledColor = Color.FromArgb(30, 0x21, 0x26);
        private Color unfilledColor = Color.FromArgb(0x25, 40, 0x31);
        private Color splitterColor = Color.FromArgb(0x3b, 0x3e, 0x47);
        private Color textColor = Color.FromArgb(120, 120, 120);
        private SortStyle sorting = SortStyle.Normal;
        private Aligning textAlignment = Aligning.Far;
        private Orientation graphOrientation = Orientation.Vertical;
        private Style graphStyle = Style.Material;
        private bool showGrid;

        // Methods
        public XUIBarGraph()
        {
            this.items.Clear();
            this.items.Add(50);
            this.items.Add(0x4b);
            this.items.Add(10);
            this.items.Add(30);
            this.items.Add(90);
            this.items.Add(60);
            this.items.Add(80);
            this.items.Add(0x2d);
            this.items.Add(70);
            this.items.Add(5);
            this.items.Add(0x19);
            this.items.Add(0x55);
            this.items.Add(0x37);
            this.items.Add(0x4b);
            base.Size = new Size(0x126, 200);
        }

        public void ClearItems()
        {
            this.items = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            if (this.items == null)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.unfilledColor), 0, 0, base.Width, base.Height);
            }
            else
            {
                if (this.graphStyle == Style.Flat)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.unfilledColor), 0, 0, base.Width, base.Height);
                    List<int> items = new List<int>();
                    if (this.sorting == SortStyle.Normal)
                    {
                        items = this.items;
                    }

                    if (this.sorting == SortStyle.Descending)
                    {
                        //Func<int, int> keySelector = <> c.<> 9__46_0;
                        //if (<> c.<> 9__46_0 == null)
                        //{
                        //    Func<int, int> local1 = <> c.<> 9__46_0;
                        //    keySelector =  <> c.<> 9__46_0 = p => p;
                        //}
                        items = this.items.OrderByDescending<int, int>(num => num).ToList<int>();
                    }

                    if (this.sorting == SortStyle.Ascending)
                    {
                        items = this.items;
                        items.Sort();
                    }

                    int y = 0;
                    if (this.graphOrientation == Orientation.Horizontal)
                    {
                        int num2 = base.Height / this.items.Count;
                        decimal num3 = base.Width / ((IEnumerable<int>) this.Items).Max();
                        foreach (int num4 in items)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(this.filledColor),
                                new RectangleF(0f, (float) y, (float) ((int) (num4 * num3)), (float) num2));
                            StringFormat format = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center
                            };
                            if (this.textAlignment == Aligning.Near)
                            {
                                format.Alignment = StringAlignment.Near;
                            }

                            if (this.textAlignment == Aligning.Center)
                            {
                                format.Alignment = StringAlignment.Center;
                            }

                            if (this.textAlignment == Aligning.Far)
                            {
                                format.Alignment = StringAlignment.Far;
                            }

                            SolidBrush brush = new SolidBrush(this.textColor);
                            RectangleF layoutRectangle =
                                new RectangleF(5f, (float) y, (float) (base.Width - 5), (float) num2);
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            e.Graphics.DrawString(num4.ToString(), this.Font, brush, layoutRectangle, format);
                            y += num2;
                        }

                        if (this.showGrid)
                        {
                            y = 0;
                            foreach (int local2 in items)
                            {
                                e.Graphics.DrawRectangle(new Pen(this.splitterColor, 1f),
                                    new Rectangle(0, y, base.Width, y + num2));
                                y += num2;
                            }

                            e.Graphics.DrawRectangle(new Pen(this.splitterColor, 1f), 1, 1, base.Width, base.Height);
                        }
                    }
                    else
                    {
                        int num5 = base.Width / this.items.Count;
                        decimal num6 = base.Height / ((IEnumerable<int>) this.Items).Max();
                        foreach (int num7 in items)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(this.filledColor),
                                new RectangleF((float) y, (float) (base.Height - ((int) (num7 * num6))), (float) num5,
                                    (float) base.Height));
                            StringFormat format = new StringFormat
                            {
                                Alignment = StringAlignment.Center
                            };
                            if (this.textAlignment == Aligning.Near)
                            {
                                format.LineAlignment = StringAlignment.Near;
                            }

                            if (this.textAlignment == Aligning.Center)
                            {
                                format.LineAlignment = StringAlignment.Center;
                            }

                            if (this.textAlignment == Aligning.Far)
                            {
                                format.LineAlignment = StringAlignment.Far;
                            }

                            SolidBrush brush = new SolidBrush(this.textColor);
                            RectangleF layoutRectangle =
                                new RectangleF((float) y, 5f, (float) num5, (float) (base.Height - 5));
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            e.Graphics.DrawString(num7.ToString(), this.Font, brush, layoutRectangle, format);
                            y += num5;
                        }

                        if (this.showGrid)
                        {
                            y = 0;
                            foreach (int local3 in items)
                            {
                                e.Graphics.DrawRectangle(new Pen(this.splitterColor, 1f),
                                    new Rectangle(y, 0, y + num5, base.Height));
                                y += num5;
                            }

                            e.Graphics.DrawRectangle(new Pen(this.splitterColor, 1f), 1, 1, base.Width, base.Height);
                        }
                    }
                }

                if (this.graphStyle == Style.Material)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(40, 40, 40)), 0, 0, base.Width, base.Height);
                    List<int> items = new List<int>();
                    if (this.sorting == SortStyle.Normal)
                    {
                        items = this.items;
                    }

                    if (this.sorting == SortStyle.Descending)
                    {
                        //Func<int, int> keySelector = <> c.<> 9__46_1;
                        //if (<> c.<> 9__46_1 == null)
                        //{
                        //    Func<int, int> local4 = <> c.<> 9__46_1;
                        //    keySelector =  <> c.<> 9__46_1 = p => p;
                        //}
                        items = this.items.OrderByDescending<int, int>(num => num).ToList<int>();
                    }

                    if (this.sorting == SortStyle.Ascending)
                    {
                        items = this.items;
                        items.Sort();
                    }

                    int num8 = 0;
                    List<Color> list3 = new List<Color>
                    {
                        Color.FromArgb(0xf9, 0x37, 0x62),
                        Color.FromArgb(0xdb, 0x37, 0x80),
                        Color.FromArgb(0xc1, 0x3a, 0x97),
                        Color.FromArgb(0xa6, 0x3a, 0xb6),
                        Color.FromArgb(0x93, 0x3d, 180),
                        Color.FromArgb(0x7e, 0x42, 0xba),
                        Color.FromArgb(0x6b, 70, 0xbc),
                        Color.FromArgb(0x4d, 0x5e, 210),
                        Color.FromArgb(0x30, 0x77, 0xe3),
                        Color.FromArgb(0x17, 0x90, 0xf9),
                        Color.FromArgb(10, 0x94, 0xf9),
                        Color.FromArgb(0, 0x98, 250),
                        Color.FromArgb(0, 0xa2, 250),
                        Color.FromArgb(0, 150, 0xd4)
                    };
                    int num9 = 0;
                    if (this.graphOrientation == Orientation.Vertical)
                    {
                        int num10 = base.Width / this.items.Count;
                        int num11 = base.Height / ((IEnumerable<int>) this.Items).Max();
                        foreach (int num12 in items)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(list3[num9]),
                                new RectangleF((float) num8, (float) (base.Height - (num12 * num11)), (float) num10,
                                    (float) base.Height));
                            StringFormat format = new StringFormat
                            {
                                Alignment = StringAlignment.Center
                            };
                            SolidBrush brush = new SolidBrush(list3[num9]);
                            RectangleF layoutRectangle = new RectangleF();
                            layoutRectangle = new RectangleF((float) num8,
                                (base.Height - (num12 * num11)) - ((this.Font.Size / 2f) * 3f), (float) num10,
                                this.Font.Size * 2f);
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            e.Graphics.DrawString(num12.ToString(), this.Font, brush, layoutRectangle, format);
                            num8 += num10;
                            num9++;
                            if (num9 == 14)
                            {
                                list3.Reverse();
                                num9 = 0;
                            }
                        }
                    }

                    if (this.graphOrientation == Orientation.Horizontal)
                    {
                        int num13 = base.Height / this.items.Count;
                        int num14 = base.Width / ((IEnumerable<int>) this.Items).Max();
                        foreach (int num15 in items)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(list3[num9]),
                                new RectangleF(0f, (float) num8, (float) (num15 * num14), (float) num13));
                            StringFormat format = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            };
                            SolidBrush brush = new SolidBrush(list3[num9]);
                            RectangleF layoutRectangle = new RectangleF();
                            layoutRectangle = new RectangleF((float) (num15 * num14), (float) num8,
                                (float) (base.Width - (num15 * num14)), (float) num13);
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            e.Graphics.DrawString(num15.ToString(), this.Font, brush, layoutRectangle, format);
                            num8 += num13;
                            num9++;
                            if (num9 == 14)
                            {
                                list3.Reverse();
                                num9 = 0;
                            }
                        }
                    }
                }

                if (this.graphStyle == Style.Bootstrap)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x23, 40, 50)), 0, 0, base.Width,
                        base.Height);
                    List<int> items = new List<int>();
                    if (this.sorting == SortStyle.Normal)
                    {
                        items = this.items;
                    }

                    if (this.sorting == SortStyle.Descending)
                    {
                        //Func<int, int> keySelector = <> c.<> 9__46_2;
                        //if (<> c.<> 9__46_2 == null)
                        //{
                        //    Func<int, int> local5 = <> c.<> 9__46_2;
                        //    keySelector =  <> c.<> 9__46_2 = p => p;
                        //}
                        items = this.items.OrderByDescending<int, int>(num => num).ToList<int>();
                    }

                    if (this.sorting == SortStyle.Ascending)
                    {
                        items = this.items;
                        items.Sort();
                    }

                    int y = 0;
                    if (this.graphOrientation == Orientation.Horizontal)
                    {
                        int num17 = base.Height / this.items.Count;
                        decimal num18 = base.Width / ((IEnumerable<int>) this.Items).Max();
                        foreach (int num19 in items)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0x23, 40)),
                                new RectangleF(0f, (float) y, (float) ((int) (num19 * num18)), (float) num17));
                            StringFormat format = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Far
                            };
                            SolidBrush brush = new SolidBrush(Color.FromArgb(0x73, 120, 0x7d));
                            RectangleF layoutRectangle =
                                new RectangleF(5f, (float) y, (float) (base.Width - 5), (float) num17);
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            e.Graphics.DrawString(num19.ToString(), this.Font, brush, layoutRectangle, format);
                            y += num17;
                        }

                        y = 0;
                        foreach (int local6 in items)
                        {
                            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(60, 0x41, 70), 1f),
                                new Rectangle(0, y, base.Width, y + num17));
                            y += num17;
                        }

                        e.Graphics.DrawRectangle(new Pen(Color.FromArgb(60, 0x41, 70), 1f), 1, 1, base.Width,
                            base.Height);
                    }
                    else
                    {
                        int num20 = base.Width / this.items.Count;
                        decimal num21 = base.Height / ((IEnumerable<int>) this.Items).Max();
                        foreach (int num22 in items)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0x23, 40)),
                                new RectangleF((float) y, (float) (base.Height - ((int) (num22 * num21))),
                                    (float) num20, (float) base.Height));
                            StringFormat format = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Near
                            };
                            SolidBrush brush = new SolidBrush(Color.FromArgb(0x73, 120, 0x7d));
                            RectangleF layoutRectangle =
                                new RectangleF((float) y, 5f, (float) num20, (float) (base.Height - 5));
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            e.Graphics.DrawString(num22.ToString(), this.Font, brush, layoutRectangle, format);
                            y += num20;
                        }

                        y = 0;
                        foreach (int local7 in items)
                        {
                            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(60, 0x41, 70), 1f),
                                new Rectangle(y, 0, y + num20, base.Height));
                            y += num20;
                        }

                        e.Graphics.DrawRectangle(new Pen(Color.FromArgb(60, 0x41, 70), 1f), 1, 1, base.Width,
                            base.Height);
                    }
                }
            }

            base.OnPaint(e);
        }

        // Properties
        [Category("XanderUI"), Browsable(true),
         Description("a collection of input numbers, will base the percentage of all numbers by the highest number")]
        public List<int> Items
        {
            get =>
                this.items;
            set
            {
                this.items = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The filled color")]
        public Color FilledColor
        {
            get =>
                this.filledColor;
            set
            {
                this.filledColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The unfilled color")]
        public Color UnfilledColor
        {
            get =>
                this.unfilledColor;
            set
            {
                this.unfilledColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The splitter color")]
        public Color SplitterColor
        {
            get =>
                this.splitterColor;
            set
            {
                this.splitterColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text color")]
        public Color TextColor
        {
            get =>
                this.textColor;
            set
            {
                this.textColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The item sorting style")]
        public SortStyle Sorting
        {
            get =>
                this.sorting;
            set
            {
                this.sorting = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The text aligning")]
        public Aligning TextAlignment
        {
            get =>
                this.textAlignment;
            set
            {
                this.textAlignment = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The orientation of the graph")]
        public Orientation GraphOrientation
        {
            get =>
                this.graphOrientation;
            set
            {
                this.graphOrientation = value;
                base.Invalidate();
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
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the item grid")]
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

        public enum Aligning
        {
            Near,
            Center,
            Far
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public enum SortStyle
        {
            Ascending,
            Descending,
            Normal
        }

        public enum Style
        {
            Flat,
            Material,
            Bootstrap
        }
    }
}
