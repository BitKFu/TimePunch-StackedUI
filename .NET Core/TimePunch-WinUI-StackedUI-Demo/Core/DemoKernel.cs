using Microsoft.UI.Xaml;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoKernel : Kernel<DemoKernel, DemoController>
    {
        public Window MainWindow { get; set; }
    }
}
