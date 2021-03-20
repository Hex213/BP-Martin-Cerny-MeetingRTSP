using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using LibUIAcademyFramework.XanderUI;

namespace LibUIAcademy.Designers
{
    public class FormDesigner : ParentControlDesigner
    {
        // Methods
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            if (this.Control is XUIFormDesign)
            {
                base.EnableDesignMode(((XUIFormDesign)this.Control).WorkingArea, "WorkingArea");
            }
        }
    }



}
