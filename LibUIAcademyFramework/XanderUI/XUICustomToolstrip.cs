using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUICustomToolstrip : ToolStrip
    {
        // Fields
        private Color borderColor = Color.DodgerBlue;

        // Methods
        public XUICustomToolstrip()
        {
            this.Dock = DockStyle.Top;
            base.Renderer = new ToolStripStripeRemoval(this.borderColor);
            base.BackColor = Color.White;
            base.ForeColor = Color.Black;
            base.GripStyle = ToolStripGripStyle.Hidden;
        }

        private void refreshUI()
        {
            base.Renderer = new ToolStripStripeRemoval(this.borderColor);
        }

        // Properties
        public Color BorderColor
        {
            get =>
                this.borderColor;
            set
            {
                this.borderColor = value;
                this.refreshUI();
            }
        }
    }


}
