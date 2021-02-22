using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibUIAcademy.Properties;

namespace LibUIAcademy.XanderUI
{
    public class XUIColorPicker : Control
    {
        // Fields
        private BufferedGraphics bufferedGraphics;
        public Image image = Resources.ColorPickerXUI;
        private Color selectedColor;
        [Category("XanderUI"), Browsable(true), Description("Show the selected color preview")]
        private bool showColorPreview = true;
        private int x1;
        private int y1;
        private bool isSelectingColor;

        // Events
        public event EventHandler SelectedColorChanged;

        // Methods
        public XUIColorPicker()
        {
            base.Size = new Size(0x85, 0x85);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        private void GetColor(int x, int y)
        {
            if ((x > 1) && ((y > 1) && ((x < (base.Width - 2)) && (y < (base.Height - 2)))))
            {
                Bitmap bitmap = (Bitmap)new Bitmap(this.image, base.Width - 3, base.Height - 3).Clone();
                if (bitmap.GetPixel(x - 1, y - 1).A > 0)
                {
                    try
                    {
                        this.selectedColor = bitmap.GetPixel(x, y);
                    }
                    catch
                    {
                    }
                }
                bitmap.Dispose();
                this.x1 = x;
                this.y1 = y;
                this.OnSelectedColorChanged();
                base.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.isSelectingColor = true;
            this.GetColor(e.X, e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            base.CreateGraphics();
            if (this.isSelectingColor)
            {
                this.GetColor(e.X, e.Y);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.isSelectingColor = false;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphicsContext current = BufferedGraphicsManager.Current;
            current.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            this.bufferedGraphics = current.Allocate(base.CreateGraphics(), base.ClientRectangle);
            this.bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            this.bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.bufferedGraphics.Graphics.Clear(this.BackColor);
            this.bufferedGraphics.Graphics.DrawImage(new Bitmap(this.image, base.Width - 3, base.Height - 3), 1, 1);
            if (this.isSelectingColor && this.showColorPreview)
            {
                this.bufferedGraphics.Graphics.FillRectangle(new SolidBrush(this.selectedColor), new RectangleF((float)(this.x1 - 10), (float)(this.y1 - 10), 20f, 20f));
            }
            this.bufferedGraphics.Render(e.Graphics);
        }

        protected virtual void OnSelectedColorChanged()
        {
            if (this.SelectedColorChanged != null)
            {
                this.SelectedColorChanged(this, new EventArgs());
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The color picker image")]
        public Image PickerImage
        {
            get =>
                this.image;
            set
            {
                this.image = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The selected color")]
        public Color SelectedColor
        {
            get =>
                this.selectedColor;
            set =>
                this.selectedColor = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Returns the selected color hex value")]
        public string SelectedColorHex =>
            ColorTranslator.ToHtml(this.selectedColor);

        public bool ShowColorPreview
        {
            get =>
                this.showColorPreview;
            set =>
                this.showColorPreview = value;
        }
    }



}
