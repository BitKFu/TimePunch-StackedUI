using TimePunch.StackedUI.Demo.ViewModels;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoViewModelLocator
    {
        private static Demo1ViewModel backingDemo1ViewModel;
        private static Demo2ViewModel backingDemo2ViewModel;
        private static Demo3ViewModel backingDemo3ViewModel;
        private static StartViewModel backingStartViewModel;
        private static MainWindowViewModel backingMainWindowViewModel;

        public static Demo1ViewModel Demo1ViewModel
        {
            get
            {
                backingDemo1ViewModel?.Dispose();
                backingDemo1ViewModel = new Demo1ViewModel();
                backingDemo1ViewModel.Initialize();
                return backingDemo1ViewModel;
            }
        }

        public static Demo2ViewModel Demo2ViewModel
        {
            get
            {
                backingDemo2ViewModel?.Dispose();
                backingDemo2ViewModel = new Demo2ViewModel();
                backingDemo2ViewModel.Initialize();
                return backingDemo2ViewModel;
            }
        }

        public static Demo3ViewModel Demo3ViewModel
        {
            get
            {
                backingDemo3ViewModel?.Dispose();
                backingDemo3ViewModel = new Demo3ViewModel();
                backingDemo3ViewModel.Initialize();
                return backingDemo3ViewModel;
            }
        }

        public static StartViewModel StartViewModel
        {
            get
            {
                backingStartViewModel?.Dispose();
                backingStartViewModel = new StartViewModel();
                backingStartViewModel.Initialize();
                return backingStartViewModel;
            }
        }

        public static MainWindowViewModel MainWindowViewModel
        {
            get
            {
                backingMainWindowViewModel?.Dispose();
                backingMainWindowViewModel = new MainWindowViewModel();
                backingMainWindowViewModel.Initialize();
                return backingMainWindowViewModel;
            }
        }

    }
}
