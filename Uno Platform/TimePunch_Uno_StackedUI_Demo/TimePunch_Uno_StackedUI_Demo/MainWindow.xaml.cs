using System.Diagnostics;
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
            Console.WriteLine("[TPUNO] MainWindow ctor start");
            Debug.WriteLine("[TPUNO] MainWindow ctor start");
            DemoKernel.Instance.MainWindow = this;
            this.InitializeComponent();

            if (DemoKernel.Instance.Controller is StackedController stackedController)
            {
                stackedController.StackedFrame = StackedFrame;
                Console.WriteLine("[TPUNO] MainWindow ctor assigned StackedFrame");
                Debug.WriteLine("[TPUNO] MainWindow ctor assigned StackedFrame");
            }

            Console.WriteLine("[TPUNO] MainWindow ctor done");
            Debug.WriteLine("[TPUNO] MainWindow ctor done");
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[TPUNO] MainWindow loaded");
            Debug.WriteLine("[TPUNO] MainWindow loaded");
            if (DataContext is MainWindowViewModel viewModel)
            {
                Console.WriteLine("[TPUNO] MainWindow loaded initialize MainWindowViewModel");
                Debug.WriteLine("[TPUNO] MainWindow loaded initialize MainWindowViewModel");
                viewModel.InitializePageAsync(this, DispatcherQueue);
            }
            else
            {
                Console.WriteLine("[TPUNO] MainWindow loaded missing MainWindowViewModel");
                Debug.WriteLine("[TPUNO] MainWindow loaded missing MainWindowViewModel");
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
