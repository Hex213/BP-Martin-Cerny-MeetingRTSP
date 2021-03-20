using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class ToolStripStripeRemoval : ToolStripSystemRenderer
    {
        // Fields
        public Color BorderColor;

        // Methods
        public ToolStripStripeRemoval(Color borderColor)
        {
            this.BorderColor = borderColor;
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(this.BorderColor, 1f), new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1));
        }
    }


}
