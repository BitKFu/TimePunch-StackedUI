using System.Windows;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Demo.Events;

namespace TimePunch.StackedUI.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DemoKernel.Instance.Controller.StackedFrame = StackedFrame;
            DemoKernel.Instance.EventAggregator.PublishMessage(new NavigateToStartView());
        }
    }
}
