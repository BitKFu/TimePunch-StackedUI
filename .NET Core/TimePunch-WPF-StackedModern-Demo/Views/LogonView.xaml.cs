using System.Windows;
using System.Windows.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Events;

namespace TimePunch_WPF_StackedModern_Demo.Views
{
    /// <summary>
    /// Interaction logic for LogonView.xaml
    /// </summary>
    public partial class LogonView : Page
    {
        public LogonView()
        {
            InitializeComponent();
        }

        private async void LogonView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var result = await LogonDialog.ShowAsync();
            await DemoKernel.Instance.EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest());
        }
    }
}
