using Microsoft.UI.Dispatching;
using TimePunch.StackedUI.ViewModels;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoViewModelBase : StackedViewModelBase
    {
        public DemoViewModelBase() : base(DemoKernel.Instance.EventAggregator)
        {
        }

        #region Overrides of ViewModelBase

        public override void Initialize()
        {
        }

        public override void InitializePage(object extraData, DispatcherQueue dispatcherQueue)
        {
            base.InitializePage(extraData, dispatcherQueue);
        }

        #endregion
    }
}
