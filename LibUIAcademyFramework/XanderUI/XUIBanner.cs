﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIBanner : Control
    {
        // Fields
        private Color borderColor = Color.White;
        private Color bannerColor = Color.FromArgb(230, 0x47, 0x59);

        // Methods
        public XUIBanner()
        {
            base.Size = new Size(100, 20);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.Text = base.Name;
            this.ForeColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            List<Point> list = new List<Point>();
            List<PointF> list2 = new List<PointF>();
            list.Add(new Point(0, (base.Height / 10) * 5));
            list.Add(new Point(base.Height / 10, (base.Height / 10) * 4));
            list.Add(new Point((base.Height / 10) * 2, (base.Height / 10) * 3));
            list.Add(new Point((base.Height / 10) * 3, (base.Height / 10) * 2));
            list.Add(new Point((base.Height / 10) * 4, base.Height / 10));
            list.Add(new Point((base.Height / 10) * 5, 0));
            list.Add(new Point(base.Width - ((base.Height / 10) * 5), 0));
            list.Add(new Point(base.Width - ((base.Height / 10) * 4), base.Height / 10));
            list.Add(new Point(base.Width - ((base.Height / 10) * 3), (base.Height / 10) * 2));
            list.Add(new Point(base.Width - ((base.Height / 10) * 2), (base.Height / 10) * 3));
            list.Add(new Point(base.Width - (base.Height / 10), (base.Height / 10) * 4));
            list.Add(new Point(base.Width, (base.Height / 10) * 5));
            list.Add(new Point(base.Width - (base.Height / 10), (base.Height / 10) * 6));
            list.Add(new Point(base.Width - ((base.Height / 10) * 2), (base.Height / 10) * 7));
            list.Add(new Point(base.Width - ((base.Height / 10) * 3), (base.Height / 10) * 8));
            list.Add(new Point(base.Width - ((base.Height / 10) * 4), (base.Height / 10) * 9));
            list.Add(new Point(base.Width - ((base.Height / 10) * 5), (base.Height / 10) * 10));
            list.Add(new Point((base.Height / 10) * 5, (base.Height / 10) * 10));
            list.Add(new Point((base.Height / 10) * 4, (base.Height / 10) * 9));
            list.Add(new Point((base.Height / 10) * 3, (base.Height / 10) * 8));
            list.Add(new Point((base.Height / 10) * 2, (base.Height / 10) * 7));
            list.Add(new Point(base.Height / 10, (base.Height / 10) * 6));
            list.Add(new Point(0, (base.Height / 10) * 5));
            SolidBrush brush = new SolidBrush(this.bannerColor);
            e.Graphics.FillPolygon(brush, list.ToArray());
            list2.Add((PointF)new Point(0, (base.Height / 10) * 5));
            list2.Add((PointF)new Point(base.Height / 10, (base.Height / 10) * 4));
            list2.Add((PointF)new Point((base.Height / 10) * 2, (base.Height / 10) * 3));
            list2.Add((PointF)new Point((base.Height / 10) * 3, (base.Height / 10) * 2));
            list2.Add((PointF)new Point((base.Height / 10) * 4, base.Height / 10));
            list2.Add((PointF)new Point((base.Height / 10) * 5, 0));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 5)) - 1, 0));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 4)) - 1, base.Height / 10));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 3)) - 1, (base.Height / 10) * 2));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 2)) - 1, (base.Height / 10) * 3));
            list2.Add((PointF)new Point((base.Width - (base.Height / 10)) - 1, (base.Height / 10) * 4));
            list2.Add((PointF)new Point(base.Width - 1, (base.Height / 10) * 5));
            list2.Add((PointF)new Point((base.Width - (base.Height / 10)) - 1, (base.Height / 10) * 6));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 2)) - 1, (base.Height / 10) * 7));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 3)) - 1, (base.Height / 10) * 8));
            list2.Add((PointF)new Point((base.Width - ((base.Height / 10) * 4)) - 1, (base.Height / 10) * 9));
            list2.Add((PointF)new Point(base.Width - ((base.Height / 10) * 5), ((base.Height / 10) * 10) - 1));
            list2.Add((PointF)new Point(((base.Height / 10) * 5) - 1, ((base.Height / 10) * 10) - 1));
            list2.Add((PointF)new Point((base.Height / 10) * 4, (base.Height / 10) * 9));
            list2.Add((PointF)new Point((base.Height / 10) * 3, (base.Height / 10) * 8));
            list2.Add((PointF)new Point((base.Height / 10) * 2, (base.Height / 10) * 7));
            list2.Add((PointF)new Point(base.Height / 10, (base.Height / 10) * 6));
            list2.Add((PointF)new Point(0, (base.Height / 10) * 5));
            Pen pen = new Pen(this.borderColor, 1f);
            e.Graphics.DrawPolygon(pen, list2.ToArray());
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            SolidBrush brush2 = new SolidBrush(this.ForeColor);
            RectangleF layoutRectangle = new RectangleF(0f, 0f, (float)base.Width, (float)base.Height);
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString(this.Text, this.Font, brush2, layoutRectangle, format);
            base.OnPaint(e);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The banner border color")]
        public Color BorderColor
        {
            get =>
                this.borderColor;
            set
            {
                this.borderColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The color of the banner")]
        public Color BannerColor
        {
            get =>
                this.bannerColor;
            set
            {
                this.bannerColor = value;
                base.Invalidate();
            }
        }
    }



}
