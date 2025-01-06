using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch_WinUI_StackedUI_Demo.ViewModels;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Model;
using TimePunch.StackedUI.Window;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimePunch_WinUI_StackedUI_Demo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Page
    {
        public MainWindow()
        {
            DemoKernel.Instance.MainWindow = this;
            this.InitializeComponent();

            if (DemoKernel.Instance.Controller is StackedController stackedController)
                stackedController.StackedFrame = StackedFrame;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.InitializePageAsync(this, DispatcherQueue);
        }


        private void OnBreadcrumbClick(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is BreadCrumbNavigation navigation)
                if (navigation.Command.CanExecute(null))
                    navigation.Command.Execute(null);
        }
    }
}
