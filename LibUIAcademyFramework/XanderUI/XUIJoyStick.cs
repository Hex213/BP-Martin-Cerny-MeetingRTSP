using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIJoyStick : Control
    {
        // Fields
        private Graphics ng;
        private Direction joystickDirection = Direction.MiddleCenter;
        private Control movableObject;
        private bool keepOnScreen = true;
        private int sensitivity = 3;
        private Image backgroundImage;
        private Color joyStickColor = Color.DodgerBlue;
        private bool moveStick;

        // Events
        public event EventHandler DirectionChanged;

        // Methods
        public XUIJoyStick()
        {
            base.Size = new Size(0x7a, 0x7a);
            this.BackColor = Color.White;
            this.BackgroundImage = new Bitmap(base.Width, base.Height);
            this.ng = base.CreateGraphics();
        }

        private void MoveObject()
        {
            if (this.movableObject != null)
            {
                if (!this.keepOnScreen)
                {
                    if (this.joystickDirection == Direction.UpperCenter)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X, this.movableObject.Location.Y - this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.UpperLeft)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X - this.sensitivity, this.movableObject.Location.Y - this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.UpperRight)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X + this.sensitivity, this.movableObject.Location.Y - this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.MiddleLeft)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X - this.sensitivity, this.movableObject.Location.Y);
                    }
                    if (this.joystickDirection == Direction.MiddleRight)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X + this.sensitivity, this.movableObject.Location.Y);
                    }
                    if (this.joystickDirection == Direction.LowerLeft)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X - this.sensitivity, this.movableObject.Location.Y + this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.LowerCenter)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X, this.movableObject.Location.Y + this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.LowerRight)
                    {
                        this.movableObject.Location = new Point(this.movableObject.Location.X + this.sensitivity, this.movableObject.Location.Y + this.sensitivity);
                    }
                }
                else
                {
                    if (this.joystickDirection == Direction.UpperLeft)
                    {
                        this.movableObject.Location = ((this.movableObject.Location.X - this.sensitivity) <= -1) ? new Point(0, this.movableObject.Location.Y) : new Point(this.movableObject.Location.X - this.sensitivity, this.movableObject.Location.Y);
                        this.movableObject.Location = ((this.movableObject.Location.Y - this.sensitivity) <= -1) ? new Point(this.movableObject.Location.X, 0) : new Point(this.movableObject.Location.X, this.movableObject.Location.Y - this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.UpperCenter)
                    {
                        this.movableObject.Location = ((this.movableObject.Location.Y - this.sensitivity) <= -1) ? new Point(this.movableObject.Location.X, 0) : new Point(this.movableObject.Location.X, this.movableObject.Location.Y - this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.UpperRight)
                    {
                        this.movableObject.Location = (((this.movableObject.Location.X + this.movableObject.Width) + this.sensitivity) >= (this.movableObject.Parent.Width - 1)) ? new Point(this.movableObject.Parent.Width - this.movableObject.Width, this.movableObject.Location.Y) : new Point(this.movableObject.Location.X + this.sensitivity, this.movableObject.Location.Y);
                        this.movableObject.Location = ((this.movableObject.Location.Y - this.sensitivity) <= -1) ? new Point(this.movableObject.Location.X, 0) : new Point(this.movableObject.Location.X, this.movableObject.Location.Y - this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.MiddleLeft)
                    {
                        this.movableObject.Location = ((this.movableObject.Location.X - this.sensitivity) <= -1) ? new Point(0, this.movableObject.Location.Y) : new Point(this.movableObject.Location.X - this.sensitivity, this.movableObject.Location.Y);
                    }
                    if (this.joystickDirection == Direction.MiddleRight)
                    {
                        this.movableObject.Location = (((this.movableObject.Location.X + this.movableObject.Width) + this.sensitivity) >= (this.movableObject.Parent.Width - 1)) ? new Point(this.movableObject.Parent.Width - this.movableObject.Width, this.movableObject.Location.Y) : new Point(this.movableObject.Location.X + this.sensitivity, this.movableObject.Location.Y);
                    }
                    if (this.joystickDirection == Direction.LowerLeft)
                    {
                        this.movableObject.Location = ((this.movableObject.Location.X - this.sensitivity) <= -1) ? new Point(0, this.movableObject.Location.Y) : new Point(this.movableObject.Location.X - this.sensitivity, this.movableObject.Location.Y);
                        this.movableObject.Location = (((this.movableObject.Location.Y + this.movableObject.Height) + this.sensitivity) >= (this.movableObject.Parent.Height - 1)) ? new Point(this.movableObject.Location.X, this.movableObject.Parent.Height - this.movableObject.Height) : new Point(this.movableObject.Location.X, this.movableObject.Location.Y + this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.LowerCenter)
                    {
                        this.movableObject.Location = (((this.movableObject.Location.Y + this.movableObject.Height) + this.sensitivity) >= (this.movableObject.Parent.Height - 1)) ? new Point(this.movableObject.Location.X, this.movableObject.Parent.Height - this.movableObject.Height) : new Point(this.movableObject.Location.X, this.movableObject.Location.Y + this.sensitivity);
                    }
                    if (this.joystickDirection == Direction.LowerRight)
                    {
                        this.movableObject.Location = (((this.movableObject.Location.X + this.movableObject.Width) + this.sensitivity) >= (this.movableObject.Parent.Width - 1)) ? new Point(this.movableObject.Parent.Width - this.movableObject.Width, this.movableObject.Location.Y) : new Point(this.movableObject.Location.X + this.sensitivity, this.movableObject.Location.Y);
                        this.movableObject.Location = (((this.movableObject.Location.Y + this.movableObject.Height) + this.sensitivity) >= (this.movableObject.Parent.Height - 1)) ? new Point(this.movableObject.Location.X, this.movableObject.Parent.Height - this.movableObject.Height) : new Point(this.movableObject.Location.X, this.movableObject.Location.Y + this.sensitivity);
                    }
                }
                this.movableObject.Refresh();
            }
        }

        protected virtual void OnDirectionChanged()
        {
            if (this.DirectionChanged != null)
            {
                this.DirectionChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.moveStick = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.moveStick)
            {
                int x = ((e.X - (((base.Width / 6) * 2) / 2)) >= 0) ? ((((e.X - (((base.Width / 6) * 2) / 2)) + ((base.Width / 6) * 2)) <= base.Width) ? (e.X - (((base.Width / 6) * 2) / 2)) : (base.Width - ((base.Width / 6) * 2))) : 0;
                int y = ((e.Y - (((base.Width / 6) * 2) / 2)) >= 0) ? ((((e.Y - (((base.Width / 6) * 2) / 2)) + ((base.Height / 6) * 2)) <= base.Height) ? (e.Y - (((base.Height / 6) * 2) / 2)) : (base.Height - ((base.Height / 6) * 2))) : 0;
                int num3 = x + (base.Width / 6);
                int num1 = y + (base.Height / 6);
                if (num1 < ((base.Height / 3) * 3))
                {
                    if (num3 < ((base.Width / 3) * 3))
                    {
                        this.joystickDirection = Direction.LowerRight;
                    }
                    if (num3 < ((base.Width / 3) * 2))
                    {
                        this.joystickDirection = Direction.LowerCenter;
                    }
                    if (num3 < (base.Width / 3))
                    {
                        this.joystickDirection = Direction.LowerLeft;
                    }
                }
                int local1 = num1;
                if (local1 < ((base.Height / 3) * 2))
                {
                    if (num3 < ((base.Width / 3) * 3))
                    {
                        this.joystickDirection = Direction.MiddleRight;
                    }
                    if (num3 < ((base.Width / 3) * 2))
                    {
                        this.joystickDirection = Direction.MiddleCenter;
                    }
                    if (num3 < (base.Width / 3))
                    {
                        this.joystickDirection = Direction.MiddleLeft;
                    }
                }
                if (local1 < (base.Height / 3))
                {
                    if (num3 < ((base.Width / 3) * 3))
                    {
                        this.joystickDirection = Direction.UpperRight;
                    }
                    if (num3 < ((base.Width / 3) * 2))
                    {
                        this.joystickDirection = Direction.UpperCenter;
                    }
                    if (num3 < (base.Width / 3))
                    {
                        this.joystickDirection = Direction.UpperLeft;
                    }
                }
                this.OnDirectionChanged();
                Image image = new Bitmap(base.Width, base.Height);
                Graphics graphics1 = Graphics.FromImage(image);
                graphics1.SmoothingMode = SmoothingMode.AntiAlias;
                graphics1.FillRectangle(new SolidBrush(this.BackColor), -1, -1, base.Width + 1, base.Height + 1);
                graphics1.DrawImage(new Bitmap(this.BackgroundImage, new Size(base.Width, base.Height)), 0, 0);
                graphics1.FillPie(new SolidBrush(this.joyStickColor), new Rectangle(x, y, (base.Width / 6) * 2, (base.Height / 6) * 2), 0f, 360f);
                this.ng.DrawImage(image, 0, 0);
                this.MoveObject();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.moveStick = false;
            this.joystickDirection = Direction.MiddleCenter;
            this.OnDirectionChanged();
            Image image = new Bitmap(base.Width, base.Height);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillRectangle(new SolidBrush(this.BackColor), -1, -1, base.Width + 1, base.Height + 1);
            graphics1.DrawImage(new Bitmap(this.BackgroundImage, new Size(base.Width, base.Height)), 0, 0);
            graphics1.FillPie(new SolidBrush(this.joyStickColor), new Rectangle((base.Width / 6) * 2, (base.Height / 6) * 2, (base.Width / 6) * 2, (base.Height / 6) * 2), 0f, 360f);
            this.ng.DrawImage(image, 0, 0);
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image image = new Bitmap(base.Width, base.Height);
            Graphics graphics1 = Graphics.FromImage(image);
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            graphics1.FillRectangle(new SolidBrush(this.BackColor), -1, -1, base.Width + 1, base.Height + 1);
            graphics1.DrawImage(new Bitmap(this.BackgroundImage, new Size(base.Width, base.Height)), 0, 0);
            graphics1.FillPie(new SolidBrush(this.joyStickColor), new Rectangle((base.Width / 6) * 2, (base.Height / 6) * 2, (base.Width / 6) * 2, (base.Height / 6) * 2), 0f, 360f);
            graphics1.DrawImage(image, 0, 0);
            e.Graphics.DrawImage(image, 0, 0);
            base.OnPaint(e);
        }

        // Properties
        public Direction JoystickDirection =>
            this.joystickDirection;

        public Control MovableObject
        {
            get =>
                this.movableObject;
            set =>
                this.movableObject = value;
        }

        public bool KeepOnScreen
        {
            get =>
                this.keepOnScreen;
            set =>
                this.keepOnScreen = value;
        }

        public int Sensitivity
        {
            get =>
                this.sensitivity;
            set
            {
                this.sensitivity = value;
                if (this.sensitivity > 10)
                {
                    this.sensitivity = 10;
                }
                if (this.sensitivity < 1)
                {
                    this.sensitivity = 1;
                }
            }
        }

        public override Image BackgroundImage
        {
            get =>
                this.backgroundImage;
            set
            {
                this.backgroundImage = value;
                this.backgroundImage = new Bitmap(this.BackgroundImage, new Size(base.Width, base.Height));
                base.Invalidate();
            }
        }

        public Color JoyStickColor
        {
            get =>
                this.joyStickColor;
            set
            {
                this.joyStickColor = value;
                base.Invalidate();
            }
        }

        // Nested Types
        public enum Direction
        {
            UpperLeft,
            MiddleLeft,
            LowerLeft,
            UpperCenter,
            MiddleCenter,
            LowerCenter,
            UpperRight,
            MiddleRight,
            LowerRight
        }
    }
}
