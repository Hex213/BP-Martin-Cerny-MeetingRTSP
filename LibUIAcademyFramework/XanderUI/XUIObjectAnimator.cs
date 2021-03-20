using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIObjectAnimator : Component
    {
        // Methods
        public void ColorAnimate(object animationObject, Color color, ColorAnimation animation, bool keepColor, int animationSpeed)
        {
            Control control = animationObject as Control;
            if (animationSpeed < 1)
            {
                animationSpeed = 1;
            }
            if (animationSpeed > 10)
            {
                animationSpeed = 10;
            }
            Graphics graphics = control.CreateGraphics();
            if (animationSpeed < 1)
            {
                animationSpeed = 1;
            }
            if (animationSpeed > 10)
            {
                animationSpeed = 10;
            }
            if (animation == ColorAnimation.FillEllipse)
            {
                int width = 1;
                int height = control.Width;
                if (control.Height > control.Width)
                {
                    height = control.Height;
                }
                height = (height + 200) + (10 * animationSpeed);
                while (width < height)
                {
                    graphics.FillEllipse(new SolidBrush(color), (control.Width / 2) - (width / 2), (control.Height / 2) - (width / 2), width, width);
                    this.WaitAnimation(10);
                    width += 10 * animationSpeed;
                }
            }
            if (animation == ColorAnimation.FillSquare)
            {
                int width = 1;
                int height = control.Width;
                if (control.Height > control.Width)
                {
                    height = control.Height;
                }
                height += 200;
                while (width < height)
                {
                    graphics.FillRectangle(new SolidBrush(color), (control.Width / 2) - (width / 2), (control.Height / 2) - (width / 2), width, width);
                    this.WaitAnimation(10);
                    width += 10 * animationSpeed;
                }
            }
            if (animation == ColorAnimation.SlideFill)
            {
                for (int i = 10; i < (control.Width + (10 * animationSpeed)); i += 10 * animationSpeed)
                {
                    graphics.FillRectangle(new SolidBrush(color), 0, 0, i, control.Height);
                    this.WaitAnimation(10);
                }
            }
            if (animation == ColorAnimation.StripeFill)
            {
                int width = 10;
                int height = (control.Height / 10) + 5;
                while (width < (control.Width + (10 * animationSpeed)))
                {
                    graphics.FillRectangle(new SolidBrush(color), 0, 0, width, height);
                    graphics.FillRectangle(new SolidBrush(color), control.Width - width, height, width, height);
                    graphics.FillRectangle(new SolidBrush(color), 0, height * 2, width, height);
                    graphics.FillRectangle(new SolidBrush(color), control.Width - width, height * 3, width, height);
                    graphics.FillRectangle(new SolidBrush(color), 0, height * 4, width, height);
                    graphics.FillRectangle(new SolidBrush(color), control.Width - width, height * 5, width, height);
                    graphics.FillRectangle(new SolidBrush(color), 0, height * 6, width, height);
                    graphics.FillRectangle(new SolidBrush(color), control.Width - width, height * 7, width, height);
                    graphics.FillRectangle(new SolidBrush(color), 0, height * 8, width, height);
                    graphics.FillRectangle(new SolidBrush(color), control.Width - width, height * 9, width, height);
                    graphics.FillRectangle(new SolidBrush(color), 0, height * 10, width, height);
                    this.WaitAnimation(10);
                    width += 10 * animationSpeed;
                }
            }
            if (animation == ColorAnimation.SplitFill)
            {
                int height = 10;
                int num9 = control.Width + (10 * animationSpeed);
                while (height < num9)
                {
                    graphics.FillRectangle(new SolidBrush(color), 0, (control.Height / 2) - (height / 2), control.Width, height);
                    this.WaitAnimation(10);
                    height += 10 * animationSpeed;
                }
            }
            this.WaitAnimation(200);
            graphics.Dispose();
            if (keepColor)
            {
                control.BackColor = color;
            }
            control.Refresh();
        }

        public void FormAnimate(Form animationForm, FormAnimation animation, int animationSpeed)
        {
            if (animationSpeed < 1)
            {
                animationSpeed = 1;
            }
            if (animationSpeed > 10)
            {
                animationSpeed = 10;
            }
            if (animation == FormAnimation.FadeIn)
            {
                animationForm.Opacity = 0.0;
                while (animationForm.Opacity < 100.0)
                {
                    animationForm.Opacity = (0.01 * animationSpeed) + animationForm.Opacity;
                    this.WaitAnimation(50);
                }
            }
            if (animation == FormAnimation.FadeOut)
            {
                animationForm.Opacity = 1.0;
                while (animationForm.Opacity > 0.1)
                {
                    animationForm.Opacity -= 0.01 * animationSpeed;
                    this.WaitAnimation(50);
                }
            }
        }

        public void StandardAnimate(object animationObject, StandardAnimation animation, int animationSpeed)
        {
            Point location;
            Control control = animationObject as Control;
            if (animationSpeed < 1)
            {
                animationSpeed = 1;
            }
            if (animationSpeed > 10)
            {
                animationSpeed = 10;
            }
            if (animation == StandardAnimation.SlideRight)
            {
                int x = control.Location.X;
                control.Location = new Point(0 - control.Width, control.Location.Y);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.X >= (x / 2))
                    {
                        while (true)
                        {
                            location = control.Location;
                            if (location.X >= (x / 4))
                            {
                                while (true)
                                {
                                    location = control.Location;
                                    if (location.X >= (x / 8))
                                    {
                                        while (true)
                                        {
                                            location = control.Location;
                                            if (location.X >= x)
                                            {
                                                control.Location = new Point(x, control.Location.Y);
                                                break;
                                            }
                                            control.Location = new Point(control.Location.X + (2 * animationSpeed), control.Location.Y);
                                            control.Refresh();
                                            this.WaitAnimation(40);
                                        }
                                        break;
                                    }
                                    control.Location = new Point(control.Location.X + (5 * animationSpeed), control.Location.Y);
                                    control.Refresh();
                                    this.WaitAnimation(40);
                                }
                                break;
                            }
                            control.Location = new Point(control.Location.X + (7 * animationSpeed), control.Location.Y);
                            control.Refresh();
                            this.WaitAnimation(40);
                        }
                        break;
                    }
                    control.Location = new Point(control.Location.X + (10 * animationSpeed), control.Location.Y);
                    control.Refresh();
                    this.WaitAnimation(40);
                }
            }
            if (animation == StandardAnimation.SlideLeft)
            {
                int x = control.Location.X;
                control.Location = new Point(control.Parent.Width + control.Width, control.Location.Y);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.X <= (x + (control.Width / 2)))
                    {
                        while (true)
                        {
                            location = control.Location;
                            if (location.X <= (x + (control.Width / 4)))
                            {
                                while (true)
                                {
                                    location = control.Location;
                                    if (location.X <= (x + (control.Width / 8)))
                                    {
                                        while (true)
                                        {
                                            location = control.Location;
                                            if (location.X <= x)
                                            {
                                                control.Location = new Point(x, control.Location.Y);
                                                break;
                                            }
                                            control.Location = new Point(control.Location.X - (2 * animationSpeed), control.Location.Y);
                                            control.Refresh();
                                            this.WaitAnimation(40);
                                        }
                                        break;
                                    }
                                    control.Location = new Point(control.Location.X - (5 * animationSpeed), control.Location.Y);
                                    control.Refresh();
                                    this.WaitAnimation(40);
                                }
                                break;
                            }
                            control.Location = new Point(control.Location.X - (7 * animationSpeed), control.Location.Y);
                            control.Refresh();
                            this.WaitAnimation(40);
                        }
                        break;
                    }
                    control.Location = new Point(control.Location.X - (10 * animationSpeed), control.Location.Y);
                    control.Refresh();
                    this.WaitAnimation(40);
                }
            }
            if (animation == StandardAnimation.SlideDown)
            {
                int y = control.Location.Y;
                control.Location = new Point(control.Location.X, 0 - control.Height);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.Y >= (y / 2))
                    {
                        while (true)
                        {
                            location = control.Location;
                            if (location.Y >= (y / 4))
                            {
                                while (true)
                                {
                                    location = control.Location;
                                    if (location.Y >= (y / 8))
                                    {
                                        while (true)
                                        {
                                            location = control.Location;
                                            if (location.Y >= y)
                                            {
                                                control.Location = new Point(control.Location.X, y);
                                                break;
                                            }
                                            control.Location = new Point(control.Location.X, control.Location.Y + (2 * animationSpeed));
                                            control.Refresh();
                                            this.WaitAnimation(40);
                                        }
                                        break;
                                    }
                                    control.Location = new Point(control.Location.X, control.Location.Y + (5 * animationSpeed));
                                    control.Refresh();
                                    this.WaitAnimation(40);
                                }
                                break;
                            }
                            control.Location = new Point(control.Location.X, control.Location.Y + (7 * animationSpeed));
                            control.Refresh();
                            this.WaitAnimation(40);
                        }
                        break;
                    }
                    control.Location = new Point(control.Location.X, control.Location.Y + (10 * animationSpeed));
                    control.Refresh();
                    this.WaitAnimation(40);
                }
            }
            if (animation == StandardAnimation.SlideUp)
            {
                int y = control.Location.Y;
                control.Location = new Point(control.Location.X, control.Parent.Height + control.Height);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.Y <= (y + (control.Height / 2)))
                    {
                        while (true)
                        {
                            location = control.Location;
                            if (location.Y <= (y + (control.Height / 4)))
                            {
                                while (true)
                                {
                                    location = control.Location;
                                    if (location.Y <= (y + (control.Height / 8)))
                                    {
                                        while (true)
                                        {
                                            location = control.Location;
                                            if (location.Y <= y)
                                            {
                                                control.Location = new Point(control.Location.X, y);
                                                break;
                                            }
                                            control.Location = new Point(control.Location.X, control.Location.Y - (2 * animationSpeed));
                                            control.Refresh();
                                            this.WaitAnimation(40);
                                        }
                                        break;
                                    }
                                    control.Location = new Point(control.Location.X, control.Location.Y - (5 * animationSpeed));
                                    control.Refresh();
                                    this.WaitAnimation(40);
                                }
                                break;
                            }
                            control.Location = new Point(control.Location.X, control.Location.Y - (7 * animationSpeed));
                            control.Refresh();
                            this.WaitAnimation(40);
                        }
                        break;
                    }
                    control.Location = new Point(control.Location.X, control.Location.Y - (10 * animationSpeed));
                    control.Refresh();
                    this.WaitAnimation(40);
                }
            }
            if (animation == StandardAnimation.SlugRight)
            {
                int x = control.Location.X;
                int width = control.Width;
                control.Location = new Point(0 - control.Width, control.Location.Y);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.X >= x)
                    {
                        control.Location = new Point(x, control.Location.Y);
                        control.Width = width;
                        control.Refresh();
                        break;
                    }
                    control.Location = new Point(control.Location.X + (8 * animationSpeed), control.Location.Y);
                    control.Refresh();
                    control.Refresh();
                    this.WaitAnimation(100 / animationSpeed);
                    while (true)
                    {
                        if (control.Width <= (width / 2))
                        {
                            while (control.Width < width)
                            {
                                control.Width += 3 * animationSpeed;
                                control.Refresh();
                                this.WaitAnimation(50 / animationSpeed);
                            }
                            break;
                        }
                        control.Width -= 3 * animationSpeed;
                        control.Refresh();
                        this.WaitAnimation(50 / animationSpeed);
                    }
                }
            }
            if (animation == StandardAnimation.SlugLeft)
            {
                int x = control.Location.X;
                int width = control.Width;
                control.Location = new Point(control.Parent.Width + control.Width, control.Location.Y);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.X <= x)
                    {
                        control.Location = new Point(x, control.Location.Y);
                        control.Width = width;
                        control.Refresh();
                        break;
                    }
                    control.Location = new Point(control.Location.X - (8 * animationSpeed), control.Location.Y);
                    control.Refresh();
                    control.Refresh();
                    this.WaitAnimation(100 / animationSpeed);
                    while (true)
                    {
                        if (control.Width <= (width / 2))
                        {
                            while (control.Width < width)
                            {
                                control.Width += 3 * animationSpeed;
                                control.Refresh();
                                this.WaitAnimation(50 / animationSpeed);
                            }
                            break;
                        }
                        control.Width -= 3 * animationSpeed;
                        control.Refresh();
                        this.WaitAnimation(50 / animationSpeed);
                    }
                }
            }
            if (animation == StandardAnimation.Hop)
            {
                int y = control.Location.Y;
                while (true)
                {
                    if (control.Location.Y <= (y - 20))
                    {
                        while (control.Location.Y < y)
                        {
                            while (true)
                            {
                                location = control.Location;
                                if (location.Y >= (y - 0x12))
                                {
                                    while (true)
                                    {
                                        location = control.Location;
                                        if (location.Y >= (y - 20))
                                        {
                                            control.Location = new Point(control.Location.X, y);
                                            break;
                                        }
                                        control.Location = new Point(control.Location.X, control.Location.Y + 6);
                                        control.Refresh();
                                        this.WaitAnimation(100 / animationSpeed);
                                    }
                                    break;
                                }
                                control.Location = new Point(control.Location.X, control.Location.Y + 2);
                                control.Refresh();
                                this.WaitAnimation(100 / animationSpeed);
                            }
                        }
                        break;
                    }
                    while (true)
                    {
                        location = control.Location;
                        if (location.Y <= (y - 10))
                        {
                            while (true)
                            {
                                location = control.Location;
                                if (location.Y <= (y - 0x12))
                                {
                                    while (true)
                                    {
                                        location = control.Location;
                                        if (location.Y <= (y - 20))
                                        {
                                            break;
                                        }
                                        control.Location = new Point(control.Location.X, control.Location.Y - 2);
                                        control.Refresh();
                                        this.WaitAnimation(100 / animationSpeed);
                                    }
                                    break;
                                }
                                control.Location = new Point(control.Location.X, control.Location.Y - 4);
                                control.Refresh();
                                this.WaitAnimation(100 / animationSpeed);
                            }
                            break;
                        }
                        control.Location = new Point(control.Location.X, control.Location.Y - 5);
                        control.Refresh();
                        this.WaitAnimation(100 / animationSpeed);
                    }
                }
            }
            if (animation == StandardAnimation.ShootRight)
            {
                int x = control.Location.X;
                int y = control.Location.Y;
                Size size = control.Size;
                control.Size = new Size(control.Width, 6);
                control.Refresh();
                control.Location = new Point(0 - control.Width, (control.Location.Y + (size.Height / 2)) - 3);
                control.Refresh();
                while (true)
                {
                    if ((control.Width - size.Width) >= (x + size.Width))
                    {
                        control.Size = new Size(x + (size.Width * 2), control.Height);
                        control.Refresh();
                        while (true)
                        {
                            location = control.Location;
                            if (location.X >= x)
                            {
                                control.Size = size;
                                control.Location = new Point(x, y);
                                control.Refresh();
                                break;
                            }
                            control.Location = new Point(control.Location.X + 0x19, control.Location.Y);
                            control.Size = new Size(control.Width - 0x19, control.Height);
                            control.Refresh();
                            this.WaitAnimation(50 / animationSpeed);
                        }
                        break;
                    }
                    control.Size = new Size(control.Width + 50, control.Height);
                    control.Refresh();
                    this.WaitAnimation(50 / animationSpeed);
                }
            }
            if (animation == StandardAnimation.ShootLeft)
            {
                int x = control.Location.X;
                int y = control.Location.Y;
                Size size = control.Size;
                control.Size = new Size(control.Width, 6);
                control.Refresh();
                control.Location = new Point(control.Parent.Width + control.Width, (control.Location.Y + (size.Height / 2)) - 3);
                control.Refresh();
                while (true)
                {
                    location = control.Location;
                    if (location.X <= x)
                    {
                        control.Size = new Size(x + (size.Width * 2), control.Height);
                        control.Refresh();
                        while (true)
                        {
                            location = control.Location;
                            if ((location.X + control.Width) <= size.Width)
                            {
                                control.Size = size;
                                control.Location = new Point(x, y);
                                control.Refresh();
                                break;
                            }
                            control.Size = new Size(control.Width - 0x19, control.Height);
                            control.Refresh();
                            this.WaitAnimation(50 / animationSpeed);
                        }
                        break;
                    }
                    control.Size = new Size(control.Width + 50, control.Height);
                    control.Location = new Point(control.Location.X - 50, control.Location.Y);
                    control.Refresh();
                    this.WaitAnimation(50 / animationSpeed);
                }
            }
        }

        private void WaitAnimation(int milliseconds)
        {
            DateTime time = DateTime.Now.AddMilliseconds((double)milliseconds);
            while (DateTime.Now < time)
            {
                Application.DoEvents();
            }
        }

        // Nested Types
        public enum ColorAnimation
        {
            FillEllipse,
            FillSquare,
            SlideFill,
            StripeFill,
            SplitFill
        }

        public enum FormAnimation
        {
            FadeIn,
            FadeOut
        }

        public enum StandardAnimation
        {
            SlideUp,
            SlideDown,
            SlideLeft,
            SlideRight,
            SlugRight,
            SlugLeft,
            Hop,
            ShootRight,
            ShootLeft
        }
    }


}
