using System.Collections.ObjectModel;
using System.Windows.Input;
using TimePunch_WPF_StackedModern_Demo.Events;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Model;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Demo.Events;

namespace TimePunch.StackedUI.Demo.ViewModels
{
    public class Demo1ViewModel : DemoViewModelBase
    {
        #region Overrides of ViewModelBase

        public override void Initialize()
        {
            NextCommand = RegisterCommand(ExecuteNextCommand, CanExecuteNextCommand, true);
            LastCommand = RegisterCommand(ExecuteLastCommand, CanExecuteLastCommand, true);

            MenuItems =
            [
                new MenuItemModel()
                {
                    Header = "File",
                    MenuItems = new ObservableCollection<MenuItemModel>()
                    {
                        new MenuItemModel() { Header = "Open", IsCheckable = true, IsChecked = true },
                        new MenuItemModel() { Header = "Close" },
                    }
                }

            ];
        }

        #endregion

        #region Next Command

        /// <summary>
        /// Gets or sets the Next command.
        /// </summary>
        /// <value>The Next command.</value>
        public ICommand NextCommand
        {
            get { return GetPropertyValue(() => NextCommand); }
            set { SetPropertyValue(() => NextCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Next command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Next command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecuteNextCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the Next command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecuteNextCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessageAsync(new NavigateToDemo3View());
        }

        #endregion

        #region Last Command

        /// <summary>
        /// Gets or sets the Last command.
        /// </summary>
        /// <value>The Last command.</value>
        public ICommand LastCommand
        {
            get { return GetPropertyValue(() => LastCommand); }
            set { SetPropertyValue(() => LastCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Last command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Last command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecuteLastCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = false;
        }

        /// <summary>
        /// Executes the Last command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecuteLastCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest());
        }

        #endregion
    }
}
