using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Demo.ViewModels;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoViewModelLocator
    {
        private static Demo1ViewModel backingDemo1ViewModel;
        private static Demo2ViewModel backingDemo2ViewModel;
        private static Demo3ViewModel backingDemo3ViewModel;
        private static Demo4ViewModel backingDemo4ViewModel;
        private static LogonViewModel backingLogonViewModel;
        private static MainWindowViewModel backingMainWindowViewModel;

        public static Demo1ViewModel Demo1ViewModel
        {
            get
            {
                backingDemo1ViewModel = new Demo1ViewModel();
                backingDemo1ViewModel.Initialize();
                return backingDemo1ViewModel;
            }
        }

        public static Demo2ViewModel Demo2ViewModel
        {
            get
            {
                backingDemo2ViewModel = new Demo2ViewModel();
                backingDemo2ViewModel.Initialize();
                return backingDemo2ViewModel;
            }
        }

        public static Demo3ViewModel Demo3ViewModel
        {
            get
            {
                backingDemo3ViewModel = new Demo3ViewModel();
                backingDemo3ViewModel.Initialize();
                return backingDemo3ViewModel;
            }
        }

        public static Demo4ViewModel Demo4ViewModel
        {
            get
            {
                backingDemo4ViewModel = new Demo4ViewModel();
                backingDemo4ViewModel.Initialize();
                return backingDemo4ViewModel;
            }
        }

        public static MainWindowViewModel MainWindowViewModel
        {
            get
            {
                backingMainWindowViewModel = new MainWindowViewModel();
                backingMainWindowViewModel.Initialize();
                return backingMainWindowViewModel;
            }
        }

        public static LogonViewModel LogonViewModel
        {
            get
            {
                backingLogonViewModel = new LogonViewModel();
                backingLogonViewModel.Initialize();
                return backingLogonViewModel;
            }
        }
    }
}
