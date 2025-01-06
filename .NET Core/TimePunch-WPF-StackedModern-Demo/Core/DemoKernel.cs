using System.Windows;
using TimePunch_WPF_StackedModern_Demo;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoKernel : Kernel<DemoKernel, DemoController>
    {
        public MainWindow MainWindow { get; set; }
    }
}
