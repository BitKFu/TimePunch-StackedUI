using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            DemoKernel.Instance.EventAggregator.PublishMessage(new GoBackPageNavigationRequest());
        }
    }
}
