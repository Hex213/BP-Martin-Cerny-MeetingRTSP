using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIFlatMenuStrip : MenuStrip
    {
        // Fields
        private Color backColor = Color.DodgerBlue;
        private Color selectedBackColor = Color.DarkOrchid;
        private Color hoverBackColor = Color.RoyalBlue;
        private Color textColor = Color.White;
        private Color hoverTextColor = Color.White;
        private Color selectedTextColor = Color.White;
        private Color seperatorColor = Color.White;

        // Methods
        public XUIFlatMenuStrip()
        {
            base.Renderer = new XUIKitMenuStripRenderer(base.BackColor, this.backColor, this.selectedBackColor, this.hoverBackColor, this.textColor, this.hoverTextColor, this.selectedTextColor, this.seperatorColor);
            base.BackColor = Color.DodgerBlue;
        }

        private void refreshUI()
        {
            base.Renderer = new XUIKitMenuStripRenderer(base.BackColor, this.backColor, this.selectedBackColor, this.hoverBackColor, this.textColor, this.hoverTextColor, this.selectedTextColor, this.seperatorColor);
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Item background color")]
        public Color ItemBackColor
        {
            get =>
                this.backColor;
            set
            {
                this.backColor = value;
                this.refreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Selected item background color")]
        public Color SelectedBackColor
        {
            get =>
                this.selectedBackColor;
            set
            {
                this.selectedBackColor = value;
                this.refreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Hover item background color")]
        public Color HoverBackColor
        {
            get =>
                this.hoverBackColor;
            set
            {
                this.hoverBackColor = value;
                this.refreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Item text color")]
        public Color TextColor
        {
            get =>
                this.textColor;
            set
            {
                this.textColor = value;
                this.refreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Hover item text color")]
        public Color HoverTextColor
        {
            get =>
                this.hoverTextColor;
            set
            {
                this.hoverTextColor = value;
                this.refreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Selected item text color")]
        public Color SelectedTextColor
        {
            get =>
                this.selectedTextColor;
            set
            {
                this.selectedTextColor = value;
                this.refreshUI();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Seperator color")]
        public Color SeperatorColor
        {
            get =>
                this.seperatorColor;
            set
            {
                this.seperatorColor = value;
                this.refreshUI();
            }
        }
    }


}
