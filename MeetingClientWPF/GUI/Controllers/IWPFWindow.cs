using System.Windows.Controls;

namespace MeetingClientWPF.GUI.Controllers
{
    public interface IWPFWindow
    {
        public void EndWinForm(bool type);
        public void StartWinForm();
        public void ShowPage(Page page);
        public void HideForm();
        public void ShowForm();
    }
}