using System;
using System.Collections.Generic;
using System.Windows.Input;
using TimePunch_WPF_StackedModern_Demo.Events;
using TimePunch.MVVM.Controller;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Events;

namespace TimePunch.StackedUI.Demo.ViewModels
{
    public class PageLink
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public Action GoToPage { get; set; }
    }


    public class MainWindowViewModel : DemoViewModelBase
    {
        #region Overrides of DemoViewModelBase

        public override void Initialize()
        {
            base.Initialize();

            Demo1Command = RegisterCommand(ExecuteDemo1Command, CanExecuteDemo1Command, true);
            GoBackNavigationPageCommand = RegisterCommand(ExecuteGoBackNavigationPageCommand, CanExecuteGoBackNavigationPageCommand, true);

            DemoPages =
            [
                new PageLink { Title = "Demo1", Icon = "Home", GoToPage = () => EventAggregator.PublishMessageAsync(new NavigateToDemo1View()) },
                new PageLink { Title = "Demo2", Icon = "AddFriend", GoToPage = () => EventAggregator.PublishMessageAsync(new NavigateToDemo2View()) }
            ];


            FooterPages =
            [
                new PageLink { Title = "Settings", Icon = "Setting", GoToPage = () => EventAggregator.PublishMessageAsync(new NavigateToSettingsView()) }
                //new() {Title = "Logon", Icon = "User", GoToPage = ()=>EventAggregator.PublishMessageAsync(new NavigateToLogonView())},
            ];
        }

        #endregion

        #region Property SelectedMenuItem

        /// <summary>
        /// Gets or sets the SelectedMenuItem.
        /// </summary>
        /// <value>The SelectedMenuItem.</value>
        public PageLink SelectedMenuItem
        {
            get { return GetPropertyValue(() => SelectedMenuItem); }
            set
            {
                if (SetPropertyValue(() => SelectedMenuItem, value))
                {
                    SelectedMenuItem.GoToPage();
                }
            }
        }

        #endregion

        #region Demo1 Command

        /// <summary>
        /// Gets or sets the Demo1 command.
        /// </summary>
        /// <value>The Demo1 command.</value>
        public ICommand Demo1Command
        {
            get { return GetPropertyValue(() => Demo1Command); }
            set { SetPropertyValue(() => Demo1Command, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Demo1 command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Demo1 command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecuteDemo1Command(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the Demo1 command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecuteDemo1Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessageAsync(new NavigateToDemo1View());
        }

        #endregion

        #region Property DemoPages

        /// <summary>
        /// Gets or sets the PageLinks.
        /// </summary>
        /// <value>The PageLinks.</value>
        public List<PageLink> DemoPages
        {
            get { return GetPropertyValue(() => DemoPages); }
            set { SetPropertyValue(() => DemoPages, value); }
        }

        #endregion

        #region Property FooterPages

        /// <summary>
        /// Gets or sets the PageLinks.
        /// </summary>
        /// <value>The PageLinks.</value>
        public List<PageLink>? FooterPages
        {
            get { return GetPropertyValue(() => FooterPages); }
            set { SetPropertyValue(() => FooterPages, value); }
        }

        #endregion

        #region GoBackNavigationPage Command

        /// <summary>
        /// Gets or sets the GoBackNavigationPage command.
        /// </summary>
        /// <value>The GoBackNavigationPage command.</value>
        public ICommand GoBackNavigationPageCommand
        {
            get { return GetPropertyValue(() => GoBackNavigationPageCommand); }
            set { SetPropertyValue(() => GoBackNavigationPageCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute GoBackNavigationPage command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute GoBackNavigationPage command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecuteGoBackNavigationPageCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the GoBackNavigationPage command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecuteGoBackNavigationPageCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (Kernel.Instance.Controller is StackedController { CanGoBackPage: true })
                EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest());
        }

        #endregion
    }
}
