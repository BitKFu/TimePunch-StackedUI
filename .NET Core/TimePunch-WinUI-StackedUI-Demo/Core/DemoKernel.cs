using Microsoft.UI.Xaml;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoKernel : Kernel
    {
        public DemoKernel(IEventAggregator eventAggregator, Window mainWindow)
        {
            EventAggregator = eventAggregator;
            MainWindow = mainWindow;
            Controller = new DemoController(EventAggregator);
        }

        public override IEventAggregator EventAggregator { get; }
        public override IBaseController Controller { get; }

        public Window MainWindow { get; }
    }
}
