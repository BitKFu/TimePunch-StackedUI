using Microsoft.UI.Dispatching;
using TimePunch.StackedUI.ViewModels;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoViewModelBase : StackedViewModelBase
    {
        public DemoViewModelBase() : base(DemoKernel.Instance.EventAggregator)
        {
        }

        public override void Initialize()
        {
        }
    }
}
