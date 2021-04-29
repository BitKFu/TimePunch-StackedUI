using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Demo.Events;

namespace TimePunch.StackedUI.Demo.ViewModels
{

    public class StartViewModel : DemoViewModelBase
    {
        #region Overrides of DemoViewModelBase

        public override void Initialize()
        {
            base.Initialize();

            Demo1Command = RegisterCommand(ExecuteDemo1Command, CanExecuteDemo1Command, true);
        }

        public override void InitializePage(object extraData)
        {
            base.InitializePage(extraData);

            DemoPages = new List<PageLink>()
            {
                new PageLink() {Title = "Demo1", GoToPage = ()=>EventAggregator.PublishMessage(new NavigateToDemo1View())},
                new PageLink() {Title = "Demo2", GoToPage = ()=>EventAggregator.PublishMessage(new NavigateToDemo2View())},
            };
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
            EventAggregator.PublishMessage(new NavigateToDemo1View());
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

        #region Property SelectedPage

        /// <summary>
        /// Gets or sets the SelectedPage.
        /// </summary>
        /// <value>The SelectedPage.</value>
        public PageLink SelectedPage
        {
            get { return GetPropertyValue(() => SelectedPage); }
            set
            {
                if (SetPropertyValue(() => SelectedPage, value))
                    SelectedPage.GoToPage();
            }
        }

        #endregion
    }
}
