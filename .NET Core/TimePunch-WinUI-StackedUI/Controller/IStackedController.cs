using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Events;

namespace TimePunch.StackedUI.Controller
{
    public interface IStackedController : IBaseController
    {
        // Adding pages
        Task<Page?> InitTopPageAsync(DispatcherQueue dispatcher, PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, bool isResizable = true, bool isModal = false);
        Task<Page?> InitSubPageAsync(DispatcherQueue dispatcher, PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, Page? basePage = null, bool isResizable = true, bool isModal = false);

        // Methods
        void HidePropertyPanel();
        void ShowPropertyPanel(UIElement content);

        // Properties
        StackedFrame? StackedFrame { get; set; }
        StackedMode StackedMode { get; }
        bool CanGoBackPage { get; }
    }
}
