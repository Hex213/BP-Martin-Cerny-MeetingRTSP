using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIPieGraph : Control
    {
        // Fields
        private Random rnd = new Random();
        public List<int> numbers = new List<int>();

        // Methods
        public XUIPieGraph()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.FromArgb(40, 40, 40);
            base.Size = new Size(100, 100);
            this.numbers.Add(5);
            this.numbers.Add(10);
            this.numbers.Add(6);
            this.numbers.Add(4);
            this.numbers.Add(9);
            this.numbers.Add(11);
            this.numbers.Add(3);
            this.numbers.Add(15);
            this.numbers.Add(12);
            this.numbers.Add(0x11);
            this.numbers.Add(3);
            this.numbers.Add(4);
            this.numbers.Add(6);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int startAngle = 0;
            List<Color> list = new List<Color> {
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
            int num2 = 0;
            int num3 = 0;
            foreach (int num4 in this.numbers)
            {
                num3++;
                if (num3 == this.numbers.Count)
                {
                    e.Graphics.FillPie(new SolidBrush(list[num2]), 0, 0, base.Width, base.Height, startAngle, 360 - startAngle);
                }
                else
                {
                    e.Graphics.FillPie(new SolidBrush(list[num2]), 0, 0, base.Width, base.Height, startAngle, (int)(num4 * 3.6));
                }
                if ((num2 + 1) == 14)
                {
                    list.Reverse();
                    num2 = 0;
                }
                startAngle += (int)(num4 * 3.6);
            }
            base.OnPaint(e);
        }

        // Properties
        public List<int> Numbers
        {
            get =>
                this.numbers;
            set
            {
                int num = 0;
                foreach (int num2 in value)
                {
                    num += num2;
                }
                if (num == 100)
                {
                    this.numbers = value;
                }
                base.Invalidate();
            }
        }
    }



}
