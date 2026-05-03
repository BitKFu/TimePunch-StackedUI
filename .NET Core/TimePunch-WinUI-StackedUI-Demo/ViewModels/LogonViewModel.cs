using System.Windows.Input;
using TimePunch.MVVM.Events;
using TimePunch_WinUI_StackedUI_Demo.Core;

namespace TimePunch_WinUI_StackedUI_Demo.ViewModels
{
    public class LogonViewModel : DemoViewModelBase
    {
        public override void Initialize()
        {
            base.Initialize();

            PrimaryButtonCommand = RegisterCommand(ExecutePrimaryButtonCommand, CanExecutePrimaryButtonCommand, true);
            CancelDialogCommand = RegisterCommand(ExecuteCancelDialogCommand, CanExecuteCancelDialogCommand, true);
        }


        #region PrimaryButton Command

        /// <summary>
        /// Gets or sets the PrimaryButton command.
        /// </summary>
        /// <value>The PrimaryButton command.</value>
        public ICommand? PrimaryButtonCommand
        {
            get { return GetPropertyValue(() => PrimaryButtonCommand); }
            set { SetPropertyValue(() => PrimaryButtonCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute PrimaryButton command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute PrimaryButton command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecutePrimaryButtonCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the PrimaryButton command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecutePrimaryButtonCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            DialogResult = true;
        }

        #endregion

        #region CancelDialog Command

        /// <summary>
        /// Gets or sets the CancelDialog command.
        /// </summary>
        /// <value>The CancelDialog command.</value>
        public ICommand? CancelDialogCommand
        {
            get { return GetPropertyValue(() => CancelDialogCommand); }
            set { SetPropertyValue(() => CancelDialogCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute CancelDialog command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute CancelDialog command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecuteCancelDialogCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the CancelDialog command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecuteCancelDialogCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            DialogResult = false;
        }

        #endregion

    }
}
