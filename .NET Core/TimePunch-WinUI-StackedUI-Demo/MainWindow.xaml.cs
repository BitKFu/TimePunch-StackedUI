using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using TimePunch_WinUI_StackedUI_Demo.Core;
using Microsoft.UI.Xaml.Controls;
using TimePunch_WinUI_StackedUI_Demo.ViewModels;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Model;
using TimePunch.StackedUI.Window;
using WindowActivatedEventArgs = Microsoft.UI.Xaml.WindowActivatedEventArgs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimePunch_WinUI_StackedUI_Demo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : StackedWindow
    {
        public MainWindow()
        {
            Kernel.Instance = new DemoKernel(new EventAggregator(), this);
            this.InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Activated += MainWindow_Activated;
            SetTitleBar(AppTitleBar);

            if (DemoKernel.Instance.Controller is StackedController stackedController)
                stackedController.StackedFrame = StackedFrame;
            
            if (ContentGrid.DataContext is MainWindowViewModel viewModel)
                viewModel.InitializePageAsync(this, DispatcherQueue);
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                TitleBarTextBlock.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                TitleBarTextBlock.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
        }

        private void OnBreadcrumbClick(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is BreadCrumbNavigation navigation)
                if (navigation.Command.CanExecute(null))
                    navigation.Command.Execute(null);
        }
    }
}
