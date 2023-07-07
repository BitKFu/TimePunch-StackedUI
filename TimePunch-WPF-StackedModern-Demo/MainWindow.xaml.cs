﻿using System;
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
using ModernWpf;
using ModernWpf.Controls;
using TimePunch.MVVM.Events;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Events;

namespace TimePunch_WPF_StackedModern_Demo
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
            SetBinding(TitleBar.IsBackButtonVisibleProperty,
                new Binding { Path = new PropertyPath(TimePunch.StackedUI.StackedFrame.CanGoBackProperty), Source = StackedFrame});
        }

        /// <summary>
        /// Bind the stacked frame to the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DemoKernel.Instance.Controller.StackedFrame = StackedFrame;
            DemoKernel.Instance.EventAggregator.PublishMessage(new NavigateToStartView());
        }

        /// <summary>
        /// Go one page back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            DemoKernel.Instance.EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest());
        }
    }
}
