using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUISlidingPanel : XUIGradientPanel
    {
        // Fields
        private XUIBackgroundSleeper sleeper = new XUIBackgroundSleeper();
        private bool collapsed = true;
        private int panelWidthExpanded = 200;
        private int panelWidthCollapsed = 50;
        private bool hideControls;
        private Control collapseControl;

        // Events
        public event EventHandler OnCollapsedStateChanged;

        // Methods
        public XUISlidingPanel()
        {
            this.Dock = DockStyle.Left;
            this.CollapseChanged();
            base.BottomRight = Color.DodgerBlue;
            base.TopLeft = Color.Black;
            base.TopRight = Color.Black;
            base.BottomLeft = Color.Black;
        }

        protected virtual void CollapsedStateChanged()
        {
            if (this.OnCollapsedStateChanged != null)
            {
                this.OnCollapsedStateChanged(this, new EventArgs());
            }
        }

        private void CollapseChanged()
        {
            if (this.collapsed)
            {
                if (this.hideControls)
                {
                    foreach (Control control in base.Controls)
                    {
                        if (!ReferenceEquals(control, this.collapseControl))
                        {
                            control.Visible = false;
                        }
                    }
                }
                while (base.Width > this.panelWidthCollapsed)
                {
                    if (base.Width > ((this.panelWidthExpanded / 5) * 3))
                    {
                        base.Size = new Size(base.Width - 30, base.Height);
                        this.sleeper.Sleep(40);
                        continue;
                    }
                    if (base.Width > ((this.panelWidthExpanded / 5) * 2))
                    {
                        base.Size = new Size(base.Width - 20, base.Height);
                        this.sleeper.Sleep(40);
                        continue;
                    }
                    base.Size = new Size(base.Width - 10, base.Height);
                    this.sleeper.Sleep(40);
                }
                base.Size = new Size(this.panelWidthCollapsed, base.Height);
            }
            else
            {
                while (base.Width < this.panelWidthExpanded)
                {
                    if (base.Width < ((this.panelWidthExpanded / 10) * 6))
                    {
                        base.Size = new Size(base.Width + 30, base.Height);
                        this.sleeper.Sleep(40);
                        continue;
                    }
                    if (base.Width < ((this.panelWidthExpanded / 10) * 4))
                    {
                        base.Size = new Size(base.Width + 20, base.Height);
                        this.sleeper.Sleep(40);
                        continue;
                    }
                    base.Size = new Size(base.Width + 10, base.Height);
                    this.sleeper.Sleep(40);
                }
                base.Size = new Size(this.panelWidthExpanded, base.Height);
                if (this.hideControls)
                {
                    foreach (Control control2 in base.Controls)
                    {
                        if (!ReferenceEquals(control2, this.collapseControl))
                        {
                            control2.Visible = true;
                        }
                    }
                }
            }
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
            if ((this.Dock != DockStyle.Left) & (this.Dock != DockStyle.Right))
            {
                this.Dock = DockStyle.Left;
            }
        }

        private void SwitchCollapsed(object sender, EventArgs e)
        {
            if (this.Collapsed)
            {
                this.Collapsed = false;
            }
            else
            {
                this.Collapsed = true;
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Is the panel collapsed")]
        public bool Collapsed
        {
            get =>
                this.collapsed;
            set
            {
                this.collapsed = value;
                this.CollapseChanged();
                this.CollapsedStateChanged();
                base.Invalidate();
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The panel width expanded")]
        public int PanelWidthExpanded
        {
            get =>
                this.panelWidthExpanded;
            set
            {
                this.panelWidthExpanded = value;
                if (!this.Collapsed)
                {
                    base.Size = new Size(this.panelWidthExpanded, base.Height);
                }
            }
        }

        [Category("XanderUI"), Browsable(true), Description("The panel width expanded")]
        public int PanelWidthCollapsed
        {
            get =>
                this.panelWidthCollapsed;
            set
            {
                this.panelWidthCollapsed = value;
                if (this.Collapsed)
                {
                    base.Size = new Size(this.panelWidthCollapsed, base.Height);
                }
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Hide controls when collapsed")]
        public bool HideControls
        {
            get =>
                this.hideControls;
            set =>
                this.hideControls = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The control used to collapse/expand the sliding panel")]
        public Control CollapseControl
        {
            get =>
                this.collapseControl;
            set
            {
                this.collapseControl = value;
                if (this.collapseControl != null)
                {
                    this.collapseControl.Click += new EventHandler(this.SwitchCollapsed);
                }
            }
        }
    }


}
