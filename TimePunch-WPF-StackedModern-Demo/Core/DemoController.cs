using System.Windows.Controls;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Demo.Views;
using TimePunch_WPF_StackedModern_Demo.Views;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoController : StackedController
        , IHandleMessage<NavigateToDemo1View>
        , IHandleMessage<NavigateToDemo2View>
        , IHandleMessage<NavigateToDemo3View>
        , IHandleMessage<NavigateToStartView>
    {
        private Page basePage;

        public DemoController() 
            : base(DemoKernel.Instance.EventAggregator, StackedMode.InPlace)
        {
        }

        #region Implementation of IHandleMessage<NavigateToDemo1View>

        public void Handle(NavigateToDemo1View message)
        {
            while (StackedFrame.TopFrame != null) StackedFrame.GoBack();
            AddPage(new Demo1View(), basePage);
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo2View>

        public void Handle(NavigateToDemo2View message)
        {
            AddPage(new Demo2View());
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo3View>

        public void Handle(NavigateToDemo3View message)
        {
            AddPage(new Demo3View());
        }

        #endregion

        #region Overrides of StackedController

        protected override Frame CreateFrame()
        {
            return new ModernWpf.Controls.Frame();
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToStartView>

        public void Handle(NavigateToStartView message)
        {
            AddPage(new LogonView());
        }

        #endregion
    }
}
