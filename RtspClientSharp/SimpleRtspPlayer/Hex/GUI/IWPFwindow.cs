using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace SimpleRtspPlayer.Hex.GUI
{
    public interface IWpFwindow
    {
        public void EndWinForm(bool type);
        public void ShowWinForm(WindowsFormsHost form);
        public void ShowPage(Page page);
        public void ShowGrid(Grid grid);

        public Grid GetMainGrid();
    }
}
