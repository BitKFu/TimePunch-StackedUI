using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using CommunityToolkit.WinUI;
using Microsoft.UI;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch_WinUI_StackedUI_Demo.Events;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Model;
using TimePunch.StackedUI.Window;
using WinRT.Interop;
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
            this.InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Activated += MainWindow_Activated;
            SetTitleBar(AppTitleBar);

            DemoKernel.Instance.Controller.StackedFrame = StackedFrame;
            
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
