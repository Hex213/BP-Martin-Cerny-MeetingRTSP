using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUISwitch : Control
    {
        // Fields
        private bool SetSwitchColor = true;
        private Style switchStyle = Style.Horizontal;
        private State switchState;
        private Color onColor = Color.FromArgb(0x66, 0xd9, 0xae);
        private Color offColor = Color.FromArgb(0xea, 0x81, 0x88);
        private Color handleOnColor = Color.FromArgb(1, 180, 120);
        private Color handleOffColor = Color.FromArgb(230, 0x47, 0x59);

        // Events
        public event EventHandler SwitchStateChanged;

        // Methods
        public XUISwitch()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            base.Size = new Size(60, 30);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.SwitchState == State.On)
            {
                this.SwitchState = State.Off;
            }
            else
            {
                this.SwitchState = State.On;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (this.switchStyle == Style.iOS)
            {
                if (this.SetSwitchColor)
                {
                    this.onColor = Color.FromArgb(0x4c, 0xd9, 100);
                    this.handleOnColor = Color.FromArgb(0xff, 0xff, 0xff);
                    this.offColor = Color.FromArgb(0xff, 0xff, 0xff);
                    this.handleOffColor = Color.FromArgb(0xff, 0xff, 0xff);
                }
                if (this.switchState == State.On)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.onColor), 15, 0, 30, 0x1d);
                    e.Graphics.FillPie(new SolidBrush(this.onColor), new Rectangle(1, 0, 30, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.onColor), new Rectangle(30, 0, 0x1d, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOnColor), new Rectangle(0x1f, 1, 0x1b, 0x1b), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOnColor), new Rectangle(0x20, 2, 0x19, 0x19), 0f, 360f);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.offColor), 15, 0, 30, 0x1d);
                    e.Graphics.FillPie(new SolidBrush(this.offColor), new Rectangle(1, 0, 30, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.offColor), new Rectangle(30, 0, 0x1d, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(200, 200, 200)), new Rectangle(2, 1, 0x1d, 0x1b), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOffColor), new Rectangle(3, 2, 0x1b, 0x19), 0f, 360f);
                }
            }
            if (this.switchStyle == Style.Android)
            {
                if (this.SetSwitchColor)
                {
                    this.onColor = Color.FromArgb(0xd9, 0xef, 0xed);
                    this.handleOnColor = Color.FromArgb(0x7e, 0xc7, 0xc0);
                    this.offColor = Color.FromArgb(0x4d, 0x4d, 0x4d);
                    this.handleOffColor = Color.FromArgb(0xb9, 0xb9, 0xb9);
                }
                if (this.switchState == State.On)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.onColor), 10, 5, 30, 20);
                    e.Graphics.FillPie(new SolidBrush(this.onColor), new Rectangle(3, 5, 20, 20), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOnColor), new Rectangle(0x19, 0, 0x1d, 0x1d), 0f, 360f);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.offColor), 10, 5, 30, 20);
                    e.Graphics.FillPie(new SolidBrush(this.offColor), new Rectangle(0x1c, 5, 20, 20), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOffColor), new Rectangle(0, 0, 0x1d, 0x1d), 0f, 360f);
                }
            }
            if (this.switchStyle == Style.Horizontal)
            {
                if (this.switchState == State.On)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.onColor), 0, 5, base.Width, base.Height - 10);
                    e.Graphics.FillRectangle(new SolidBrush(this.handleOnColor), (base.Width / 2) + 2, 7, (base.Width / 2) - 5, base.Height - 14);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.offColor), 0, 5, base.Width, base.Height - 10);
                    e.Graphics.FillRectangle(new SolidBrush(this.handleOffColor), 2, 7, (base.Width / 2) - 5, base.Height - 14);
                }
            }
            if (this.switchStyle == Style.Vertical)
            {
                if (this.switchState == State.On)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.onColor), 5, 0, base.Width - 10, base.Height);
                    e.Graphics.FillRectangle(new SolidBrush(this.handleOnColor), 7, (base.Height / 2) + 2, base.Width - 14, (base.Height / 2) - 5);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.offColor), 5, 0, base.Width - 10, base.Height);
                    e.Graphics.FillRectangle(new SolidBrush(this.handleOffColor), 7, 2, base.Width - 14, (base.Height / 2) - 5);
                }
            }
            if (this.switchStyle == Style.Dark)
            {
                if (this.SetSwitchColor)
                {
                    this.onColor = Color.FromArgb(40, 40, 40);
                    this.handleOnColor = Color.FromArgb(0xff, 0xff, 0xff);
                    this.offColor = Color.FromArgb(0x4b, 0x4b, 0x4b);
                    this.handleOffColor = Color.FromArgb(0xff, 0xff, 0xff);
                }
                if (this.switchState == State.On)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.onColor), 15, 0, 30, 0x1d);
                    e.Graphics.FillPie(new SolidBrush(this.onColor), new Rectangle(1, 0, 30, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.onColor), new Rectangle(30, 0, 0x1d, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(200, 200, 200)), new Rectangle(0x1f, 1, 0x1b, 0x1b), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOnColor), new Rectangle(0x20, 2, 0x19, 0x19), 0f, 360f);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.offColor), 15, 0, 30, 0x1d);
                    e.Graphics.FillPie(new SolidBrush(this.offColor), new Rectangle(1, 0, 30, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.offColor), new Rectangle(30, 0, 0x1d, 0x1d), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(200, 200, 200)), new Rectangle(2, 1, 0x1d, 0x1b), 0f, 360f);
                    e.Graphics.FillPie(new SolidBrush(this.handleOffColor), new Rectangle(3, 2, 0x1b, 0x19), 0f, 360f);
                }
            }
            base.OnPaint(e);
        }

        protected virtual void OnSwitchStateChanged()
        {
            if (this.SwitchStateChanged != null)
            {
                this.SwitchStateChanged(this, new EventArgs());
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The text of the button")]
        public Style SwitchStyle
        {
            get =>
                this.switchStyle;
            set
            {
                this.switchStyle = value;
                this.SetSwitchColor = true;
                if (value == Style.iOS)
                {
                    base.Size = new Size(60, 30);
                }
                if (value == Style.Android)
                {
                    base.Size = new Size(0x3a, 30);
                }
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The smoothing mode of the graphics")]
        public State SwitchState
        {
            get =>
                this.switchState;
            set
            {
                this.switchState = value;
                this.OnSwitchStateChanged();
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The button on color")]
        public Color OnColor
        {
            get =>
                this.onColor;
            set
            {
                this.onColor = value;
                this.SetSwitchColor = false;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The button off color")]
        public Color OffColor
        {
            get =>
                this.offColor;
            set
            {
                this.offColor = value;
                this.SetSwitchColor = false;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The button on color")]
        public Color HandleOnColor
        {
            get =>
                this.handleOnColor;
            set
            {
                this.handleOnColor = value;
                this.SetSwitchColor = false;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The button off color")]
        public Color HandleOffColor
        {
            get =>
                this.handleOffColor;
            set
            {
                this.handleOffColor = value;
                this.SetSwitchColor = false;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum State
        {
            On,
            Off
        }

        public enum Style
        {
            Vertical,
            Horizontal,
            iOS,
            Android,
            Dark
        }
    }



}
