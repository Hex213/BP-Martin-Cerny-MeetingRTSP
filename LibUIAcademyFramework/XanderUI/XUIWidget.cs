using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibUIAcademy.XanderUI
{
    public class XUIWidget
    {
        // Fields
        private bool isDragging;

        // Methods
        public void SetWidget(Control C)
        {
            C.MouseDown += new MouseEventHandler(this.WidgetDown);
            C.MouseUp += new MouseEventHandler(this.WidgetUp);
            C.MouseMove += new MouseEventHandler(this.WidgetMove);
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
    }



}
