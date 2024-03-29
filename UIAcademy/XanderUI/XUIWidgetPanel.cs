﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIWidgetPanel : Panel
    {
        // Fields
        private bool controlsAsWidgets;
        private bool isDragging;

        // Methods
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (!this.ControlsAsWidgets)
            {
                foreach (Control control1 in base.Controls)
                {
                    control1.MouseDown += new MouseEventHandler(this.WidgetDown);
                    control1.MouseUp += new MouseEventHandler(this.WidgetUp);
                    control1.MouseMove += new MouseEventHandler(this.WidgetMove);
                }
            }
        }

        private void WidgetDown(object sender, MouseEventArgs e)
        {
            this.isDragging = true;
        }

        private void WidgetMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                ((Control)sender).Location = new Point(e.X, e.Y);
            }
        }

        private void WidgetUp(object sender, MouseEventArgs e)
        {
            this.isDragging = false;
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("reat controls as widgets")]
        public bool ControlsAsWidgets
        {
            get =>
                this.controlsAsWidgets;
            set =>
                this.controlsAsWidgets = value;
        }
    }



}
