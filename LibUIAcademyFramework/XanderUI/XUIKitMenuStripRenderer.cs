using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIKitMenuStripRenderer : ToolStripRenderer
    {
        // Fields
        public Color headerColor;
        public Color backColor;
        public Color selectedBackColor;
        public Color hoverBackColor;
        public Color textColor;
        public Color hoverTextColor;
        public Color selectedTextColor;
        public Color seperatorColor;

        // Methods
        public XUIKitMenuStripRenderer(Color hc, Color bc, Color sbc, Color hbc, Color tc, Color htc, Color stc, Color sc)
        {
            this.headerColor = hc;
            this.backColor = bc;
            this.selectedBackColor = sbc;
            this.hoverBackColor = hbc;
            this.textColor = tc;
            this.hoverTextColor = htc;
            this.selectedTextColor = stc;
            this.seperatorColor = sc;
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);
            Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
            e.Graphics.FillRectangle(new SolidBrush(this.backColor), rect);
            SolidBrush brush = new SolidBrush(this.backColor);
            Rectangle rectangle2 = new Rectangle(0, 0, 0x1a, e.AffectedBounds.Height);
            e.Graphics.FillRectangle(brush, rectangle2);
            e.Graphics.DrawLine(new Pen(new SolidBrush(this.backColor)), 0x1c, 0, 0x1c, e.AffectedBounds.Height);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);
            if (e.Item.Selected)
            {
                Rectangle rect = new Rectangle(4, 2, 0x12, 0x12);
                Rectangle rectangle2 = new Rectangle(5, 3, 0x10, 0x10);
                SolidBrush brush = new SolidBrush(this.selectedTextColor);
                SolidBrush brush2 = new SolidBrush(this.selectedBackColor);
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.FillRectangle(brush2, rectangle2);
                e.Graphics.DrawImage(e.Image, new Point(5, 3));
            }
            else
            {
                Rectangle rect = new Rectangle(4, 2, 0x12, 0x12);
                Rectangle rectangle4 = new Rectangle(5, 3, 0x10, 0x10);
                SolidBrush brush = new SolidBrush(this.textColor);
                SolidBrush brush4 = new SolidBrush(this.backColor);
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.FillRectangle(brush4, rectangle4);
                e.Graphics.DrawImage(e.Image, new Point(5, 3));
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);
            if (e.Item.Enabled)
            {
                if (e.Item.IsOnDropDown || !e.Item.Selected)
                {
                    e.Item.ForeColor = this.textColor;
                }
                else
                {
                    Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                    e.Graphics.FillRectangle(new SolidBrush(this.hoverBackColor), rect);
                    e.Item.ForeColor = this.hoverTextColor;
                }
                if (e.Item.IsOnDropDown && e.Item.Selected)
                {
                    Rectangle rect = new Rectangle(1, -4, e.Item.Width + 5, e.Item.Height + 4);
                    e.Graphics.FillRectangle(new SolidBrush(this.hoverBackColor), rect);
                    e.Item.ForeColor = this.textColor;
                }
                if ((e.Item as ToolStripMenuItem).DropDown.Visible && !e.Item.IsOnDropDown)
                {
                    Rectangle rect = new Rectangle(0, 0, e.Item.Width + 3, e.Item.Height);
                    e.Graphics.FillRectangle(new SolidBrush(this.selectedBackColor), rect);
                    e.Item.ForeColor = this.selectedTextColor;
                }
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
            SolidBrush brush = new SolidBrush(this.seperatorColor);
            Rectangle rect = new Rectangle(1, 3, e.Item.Width, 1);
            e.Graphics.FillRectangle(brush, rect);
        }
    }


}
