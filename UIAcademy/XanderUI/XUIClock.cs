using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIClock : Control
    {
        // Fields
        private Timer RefreshUI = new Timer();
        private BufferedGraphics bufferedGraphics;
        private int circleThickness = 6;
        private Color unfilledHourColor = Color.FromArgb(0x4b, 70, 0x55);
        private Color filledHourColor = Color.FromArgb(0x69, 190, 0x9b);
        private Color unfilledMinuteColor = Color.FromArgb(60, 60, 70);
        private Color filledMinuteColor = Color.DodgerBlue;
        private Color unfilledSecondColor = Color.FromArgb(60, 60, 70);
        private Color filledSecondColor = Color.DarkOrchid;
        private Color hexagonColor = Color.FromArgb(60, 60, 70);
        private bool showSecondsCircle = true;
        private bool showMinutesCircle = true;
        private bool showHexagon = true;
        private bool showAMPM;
        private HourFormat displayFormat;

        // Methods
        public XUIClock()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.Size = new Size(120, 130);
            this.Font = new Font("Impact", 15f);
            this.RefreshUI.Interval = 0x3e8;
            this.RefreshUI.Tick += new EventHandler(this.RefreshUI_Tick);
            this.RefreshUI.Enabled = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rectangle;
            BufferedGraphicsContext current = BufferedGraphicsManager.Current;
            current.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), base.ClientRectangle);
            this.bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            this.bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.bufferedGraphics.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.bufferedGraphics.Graphics.Clear(this.BackColor);
            if (this.ShowHexagon)
            {
                List<Point> list = new List<Point> {
                new Point(0, base.Height / 4),
                new Point(base.Width / 2, 0),
                new Point(base.Width, base.Height / 4),
                new Point(base.Width, (base.Height / 4) * 3),
                new Point(base.Width / 2, base.Height),
                new Point(0, (base.Height / 4) * 3),
                new Point(0, base.Height / 4)
            };
                this.bufferedGraphics.Graphics.FillPolygon(new SolidBrush(this.hexagonColor), list.ToArray());
            }
            int num = (int)Math.Round((double)(((double)(DateTime.Now.Hour * 100)) / 0x18));
            int num2 = (int)Math.Round((double)(((double)(DateTime.Now.Minute * 100)) / 60.0));
            int num3 = (int)Math.Round((double)(((double)(DateTime.Now.Second * 100)) / 60.0));
            if (this.showSecondsCircle && this.showMinutesCircle)
            {
                rectangle = new Rectangle(((base.Width / 8) + (this.circleThickness * 2)) - 2, ((base.Height / 6) + (this.circleThickness * 2)) - 1, (((base.Width / 8) * 6) - (this.circleThickness * 4)) + 4, (((base.Height / 6) * 4) - (this.circleThickness * 4)) + 2);
                this.bufferedGraphics.Graphics.DrawArc(new Pen(this.unfilledSecondColor, (float)this.circleThickness), rectangle, 270f, 360f);
                this.bufferedGraphics.Graphics.DrawArc(new Pen(this.filledSecondColor, (float)this.circleThickness), rectangle, 270f, (float)((int)(num3 * 3.6)));
            }
            if (this.showMinutesCircle)
            {
                rectangle = new Rectangle(((base.Width / 8) + this.circleThickness) - 1, ((base.Height / 6) + this.circleThickness) - 1, (((base.Width / 8) * 6) - (this.circleThickness * 2)) + 2, (((base.Height / 6) * 4) - (this.circleThickness * 2)) + 2);
                this.bufferedGraphics.Graphics.DrawArc(new Pen(this.unfilledMinuteColor, (float)this.circleThickness), rectangle, 270f, 360f);
                this.bufferedGraphics.Graphics.DrawArc(new Pen(this.filledMinuteColor, (float)this.circleThickness), rectangle, 270f, (float)((int)(num2 * 3.6)));
            }
            rectangle = new Rectangle(base.Width / 8, base.Height / 6, (base.Width / 8) * 6, (base.Height / 6) * 4);
            this.bufferedGraphics.Graphics.DrawArc(new Pen(this.unfilledHourColor, (float)this.circleThickness), rectangle, 270f, 360f);
            this.bufferedGraphics.Graphics.DrawArc(new Pen(this.filledHourColor, (float)this.circleThickness), rectangle, 270f, (float)((int)(num * 3.6)));
            rectangle.Inflate(0, -5);
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            if (this.displayFormat != HourFormat.TwelveHour)
            {
                this.bufferedGraphics.Graphics.DrawString(DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString(), this.Font, new SolidBrush(Color.FromArgb(220, 220, 220)), rectangle, format);
            }
            else if (this.showAMPM)
            {
                this.bufferedGraphics.Graphics.DrawString(DateTime.Now.ToString("h:mm") + "\n" + DateTime.Now.ToString("tt"), this.Font, new SolidBrush(Color.FromArgb(220, 220, 220)), rectangle, format);
            }
            else
            {
                this.bufferedGraphics.Graphics.DrawString(DateTime.Now.ToString("h:mm"), this.Font, new SolidBrush(Color.FromArgb(220, 220, 220)), rectangle, format);
            }
            this.bufferedGraphics.Render(e.Graphics);
            base.OnPaint(e);
        }

        private void RefreshUI_Tick(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The circle thickness")]
        public int CircleThickness
        {
            get =>
                this.circleThickness;
            set
            {
                this.circleThickness = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The unfilled hour circle color")]
        public Color UnfilledHourColor
        {
            get =>
                this.unfilledHourColor;
            set
            {
                this.unfilledHourColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The filled hour circle color")]
        public Color FilledHourColor
        {
            get =>
                this.filledHourColor;
            set
            {
                this.filledHourColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The unfilled minute circle color")]
        public Color UnfilledMinuteColor
        {
            get =>
                this.unfilledMinuteColor;
            set
            {
                this.unfilledMinuteColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The filled minute circle color")]
        public Color FilledMinuteColor
        {
            get =>
                this.unfilledMinuteColor;
            set
            {
                this.unfilledMinuteColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The unfilled second circle color")]
        public Color UnfilledSecondColor
        {
            get =>
                this.unfilledSecondColor;
            set
            {
                this.unfilledSecondColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The filled second circle color")]
        public Color FilledSecondColor
        {
            get =>
                this.filledSecondColor;
            set
            {
                this.filledSecondColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The hexagon color")]
        public Color HexagonColor
        {
            get =>
                this.hexagonColor;
            set
            {
                this.hexagonColor = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the seconds circle")]
        public bool ShowSecondsCircle
        {
            get =>
                this.showSecondsCircle;
            set
            {
                this.showSecondsCircle = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the minutes circle")]
        public bool ShowMinutesCircle
        {
            get =>
                this.showMinutesCircle;
            set
            {
                this.showMinutesCircle = value;
                if (value)
                {
                    this.ShowSecondsCircle = false;
                }
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the hexagon")]
        public bool ShowHexagon
        {
            get =>
                this.showHexagon;
            set
            {
                this.showHexagon = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show AM/PM")]
        public bool ShowAmPm
        {
            get =>
                this.showAMPM;
            set
            {
                this.showAMPM = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The time display format")]
        public HourFormat DisplayFormat
        {
            get =>
                this.displayFormat;
            set
            {
                this.displayFormat = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum HourFormat
        {
            TwelveHour,
            TwentyFourHour
        }
    }



}
