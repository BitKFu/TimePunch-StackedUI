using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Model;
using TimePunch_WinUI_StackedUI_Demo.Events;
using TimePunch_WinUI_StackedUI_Demo.ViewModels;
using TimePunch_WinUI_StackedUI_Demo.Views;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoController : StackedController
        , IHandleMessageAsync<NavigateToDemo1View>
        , IHandleMessageAsync<NavigateToDemo2View>
        , IHandleMessageAsync<NavigateToDemo3View>
        , IHandleMessageAsync<NavigateToDemo4View>
        , IHandleMessageAsync<NavigateToSettingsView>
        , IHandleMessageAsync<NavigateToLogonView>
    {
        private readonly IPagePersister demoPagePersister = new DemoPagePersister();

        public DemoController() 
            : base(DemoKernel.Instance.EventAggregator)
        {
        }

        #region Implementation of IHandleMessage<NavigateToDemo1View>

        public async Task<NavigateToDemo1View> Handle(NavigateToDemo1View message)
        {
            var page = new Demo1View();
            if (page.DataContext is Demo1ViewModel viewModel)
            {
                await InitTopPageAsync(DispatcherQueue.GetForCurrentThread(), message, viewModel, page);
            }

            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo2View>

        public async Task<NavigateToDemo2View> Handle(NavigateToDemo2View message)
        {
            var page = new Demo2View();
            if (page.DataContext is Demo2ViewModel viewModel)
            {
                await InitTopPageAsync(DispatcherQueue.GetForCurrentThread(), message, viewModel, page);
            }

            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo3View>

        public async Task<NavigateToDemo3View> Handle(NavigateToDemo3View message)
        {
            var page = new Demo3View();
            if (page.DataContext is Demo3ViewModel viewModel)
            {
                await InitSubPageAsync(DispatcherQueue.GetForCurrentThread(), message, viewModel, page);
            }

            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo4View>

        public async Task<NavigateToDemo4View> Handle(NavigateToDemo4View message)
        {
            var page = new Demo4View();
            if (page.DataContext is Demo4ViewModel viewModel)
            {
                await InitSubPageAsync(DispatcherQueue.GetForCurrentThread(), message, viewModel, page);
            }

            return message;
        }

        #endregion

        #region Overrides of StackedController

        protected override Frame CreateFrame()
        {
            return new Frame();
        }

        protected override IPagePersister? GetPagePersister()
        {
            // here we could offer a class to persist the page size
            return demoPagePersister;
        }

        protected override void UpdatePropertyPanels(Page newTopPage)
        {
            // here we could disable/enable property panels
        }

        protected override void SetPageFocus(Page addedPage)
        {
            // here we could update the focus of the added page
            base.SetPageFocus(addedPage);
        }

        #endregion


        public async Task<NavigateToSettingsView> Handle(NavigateToSettingsView message)
        {
            var page = new SettingsView();
            if (page.DataContext is SettingsViewModel viewModel)
            {
                await InitTopPageAsync(DispatcherQueue.GetForCurrentThread(), message, viewModel, page);
            }
            return message;
        }

        public async Task<NavigateToLogonView> Handle(NavigateToLogonView message)
        {
            await GoBackPageTop();

            // Show the logon dialog
            var logOn = new LogonDialog()
            {
                XamlRoot = StackedFrame?.XamlRoot
            };
            
            if (logOn.DataContext is LogonViewModel vm && logOn.DialogResult == null)
            {
                if (await vm.InitializePageAsync(message, DispatcherQueue.GetForCurrentThread()))
                {
                    await logOn.ShowAsync();
                }
            }

            return message;
        }
    }
}
