using System.Threading.Tasks;
using System.Windows.Controls;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Demo.Views;
using TimePunch_WPF_StackedModern_Demo.Views;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoController : StackedController
        , IHandleMessageAsync<NavigateToDemo1View>
        , IHandleMessageAsync<NavigateToDemo2View>
        , IHandleMessageAsync<NavigateToDemo3View>
        , IHandleMessageAsync<NavigateToStartView>
    {
        private Page basePage;

        public DemoController() 
            : base(DemoKernel.Instance.EventAggregator, StackedMode.FullWidth)
        {
        }

        #region Implementation of IHandleMessage<NavigateToDemo1View>

        public async Task<NavigateToDemo1View> Handle(NavigateToDemo1View message)
        {
            while (StackedFrame.TopFrame != null)
                await StackedFrame.GoBack(false);
            await AddPage(new Demo1View(), basePage);
            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo2View>

        public async Task<NavigateToDemo2View> Handle(NavigateToDemo2View message)
        {
            await AddPage(new Demo2View());
            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo3View>

        public async Task<NavigateToDemo3View> Handle(NavigateToDemo3View message)
        {
            await AddPage(new Demo3View());
            return message;
        }

        #endregion

        #region Overrides of StackedController

        protected override Frame CreateFrame()
        {
            return new ModernWpf.Controls.Frame();
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToStartView>

        public async Task<NavigateToStartView> Handle(NavigateToStartView message)
        {
            await AddPage(new LogonView());
            return message;
        }

        #endregion
    }
}
