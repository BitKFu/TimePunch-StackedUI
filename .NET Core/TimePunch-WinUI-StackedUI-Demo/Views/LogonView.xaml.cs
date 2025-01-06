using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.StackedUI.Events;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch.MVVM.Controller;

namespace TimePunch_WinUI_StackedUI_Demo.Views
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

        private async void LogonView_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var logonDialog = new LogonDialog()
            {
                XamlRoot = this.XamlRoot
            };
            var result = await logonDialog.ShowAsync();
            await DemoKernel.Instance.EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest());
        }
    }
}
