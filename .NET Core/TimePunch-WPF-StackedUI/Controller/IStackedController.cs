using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Events;

namespace TimePunch.StackedUI.Controller
{
    public interface IStackedController : IBaseController
    {
        // Adding pages
        Task<Page?> InitTopPageAsync(PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, bool isResizable = true, bool isModal = false);
        Task<Page?> InitSubPageAsync(PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, Page? basePage = null, bool isResizable = true, bool isModal = false);

        // Methods
        void HidePropertyPanel();
        void ShowPropertyPanel(UIElement content);

        // Properties
        StackedFrame? StackedFrame { get; set; }
        StackedMode StackedMode { get; }
        bool CanGoBackPage { get; }
    }
}
