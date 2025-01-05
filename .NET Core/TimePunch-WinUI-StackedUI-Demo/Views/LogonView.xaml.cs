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
            var result = await LogonDialog.ShowAsync();
            await Kernel.Instance.EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest());
        }
    }
}
