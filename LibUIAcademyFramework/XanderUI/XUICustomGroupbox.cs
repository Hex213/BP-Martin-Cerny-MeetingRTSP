using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUICustomGroupbox : GroupBox
    {
        // Fields
        private Label groupName = new Label();
        private Color borderColor = Color.DodgerBlue;
        private Color textColor = Color.DodgerBlue;
        private int borderWidth = 1;
        private bool showText = true;

        // Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.showText)
            {
                base.Controls.Remove(this.groupName);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), 1, 1, base.Width - 2, 1);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), 1, base.Height - 2, base.Width - 2, base.Height - 2);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), 1, 1, 1, base.Height - 2);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), base.Width - 2, 1, base.Width - 2, base.Height - 2);
            }
            else
            {
                this.groupName.BackColor = Color.Transparent;
                this.groupName.Text = this.Text;
                this.groupName.Location = new Point(9, 0);
                this.groupName.AutoSize = true;
                this.groupName.ForeColor = this.textColor;
                base.Controls.Add(this.groupName);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), 1, 6, 6, 6);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), base.Width - 2, 6, this.groupName.Location.X + this.groupName.Width, 6);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), 1, base.Height - 2, base.Width - 2, base.Height - 2);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), 1, 6, 1, base.Height - 2);
                e.Graphics.DrawLine(new Pen(this.borderColor, (float)this.borderWidth), base.Width - 2, 6, base.Width - 2, base.Height - 2);
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The color of the border")]
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

        [Category("XanderUI"), Browsable(true), Description("The color of the text")]
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

        [Category("XanderUI"), Browsable(true), Description("The width of the border")]
        public int BorderWidth
        {
            get =>
                this.borderWidth;
            set
            {
                this.borderWidth = value;
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Show the text of the groupbox")]
        public bool ShowText
        {
            get =>
                this.showText;
            set
            {
                this.showText = value;
                base.Invalidate();
            }
        }
    }



}
