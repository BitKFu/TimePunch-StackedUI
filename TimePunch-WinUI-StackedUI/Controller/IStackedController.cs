using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.MVVM.Controller;

namespace TimePunch.StackedUI.Controller
{
    public interface IStackedController : IBaseController
    {
        // Methods
        Task<Page?> AddPage(Page page, Page? basePage = null, bool isResizable = true, bool isModal = true);
        void HidePropertyPanel();
        void ShowPropertyPanel(UIElement content);

        // Properties
        StackedFrame? StackedFrame { get; set; }
        StackedMode StackedMode { get; }
        bool CanGoBackPage { get; }
    }

}
