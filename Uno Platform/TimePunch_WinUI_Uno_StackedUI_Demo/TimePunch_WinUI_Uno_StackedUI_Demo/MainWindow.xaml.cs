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

    private async void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DemoKernel.Instance.Controller is StackedController stackedController)
            stackedController.StackedFrame = StackedFrame;
        
        if (ContentGrid.DataContext is MainWindowViewModel viewModel)
            await viewModel.InitializePageAsync(this, DispatcherQueue);
    }

    private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (ContentGrid.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (args.InvokedItemContainer?.DataContext is PageLink pageLink)
        {
            viewModel.SelectedMenuItem = pageLink;
        }
    }
}
