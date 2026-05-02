using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.StackedUI.Controller;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch_WinUI_StackedUI_Demo.ViewModels;

namespace TimePunch_WinUI_StackedUI_Demo;

public sealed partial class MainWindow : UserControl
{
    public MainWindow()
    {
        InitializeComponent();
        DemoKernel.Instance.MainWindow = this;
    }

    private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DemoKernel.Instance.Controller is StackedController stackedController)
        {
            stackedController.StackedFrame = StackedFrame;
        }

        if (ContentGrid.DataContext is MainWindowViewModel viewModel)
        {
            _ = viewModel.InitializePageAsync(this, DispatcherQueue);
        }
    }
}
