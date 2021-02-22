using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace SimpleRtspPlayer.Hex.Engine.GUI
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
