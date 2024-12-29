using System.Windows;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoKernel : Kernel
    {
        public DemoKernel(IEventAggregator eventAggregator, Window mainWindow)
        {
            EventAggregator = eventAggregator;
            MainWindow = mainWindow;
            Controller = new DemoController(eventAggregator);
        }

        public override IEventAggregator EventAggregator { get; }
        public override IBaseController Controller { get; }

        public Window MainWindow { get; }
    }
}
