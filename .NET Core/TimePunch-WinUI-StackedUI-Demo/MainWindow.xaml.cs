using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Model;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch_WinUI_StackedUI_Demo.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimePunch_WinUI_StackedUI_Demo
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            DemoKernel.Instance.MainWindow = this;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DemoKernel.Instance.Controller is StackedController stackedController)
                stackedController.StackedFrame = StackedFrame;
            
            if (ContentGrid.DataContext is MainWindowViewModel viewModel)
                _ = viewModel.InitializePageAsync(this, DispatcherQueue);
        }

        private void OnBreadcrumbClick(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is BreadCrumbNavigation navigation)
                if (navigation.Command.CanExecute(null))
                    navigation.Command.Execute(null);
        }
    }
}
