using System.Collections.ObjectModel;
using System.Windows.Input;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Model;
#pragma warning disable CA1416

namespace TimePunch.StackedUI.ViewModels
{
    public abstract class StackedViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Creates a new instance of the StackedViewModelBase
        /// </summary>
        /// <param name="eventAggregator"></param>
        protected StackedViewModelBase(IEventAggregator eventAggregator) 
            : base(eventAggregator)
        {
        }

        #region Property MenuItems

        /// <summary>
        /// Gets or sets the $Property$.
        /// </summary>
        /// <value>The $Property$.</value>
        public ObservableCollection<MenuItemModel>? MenuItems
        {
            get
            {
                return GetPropertyValue(() => MenuItems);
            }
            set 
            { 
                SetPropertyValue(() => MenuItems, value);
            }
        }

        #endregion


        #region Property LastFiredCommand

        /// <summary>
        /// Gets or sets the LastFiredCommand.
        /// </summary>
        /// <value>The LastFiredCommand.</value>
        protected ICommand? LastFiredCommand { get; set; }

        private object? lastFiredCommandSender;

        /// <summary>
        /// Resets the last fired command
        /// </summary>
        public virtual void ResetLastFiredCommand()
        {
            LastFiredCommand = null;
            lastFiredCommandSender = null;
        }

        /// <summary>
        /// Set the last fired command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="command"></param>
        /// <returns>Returns true, if the navigation shall take place. It returns false, if the app shall go one step back</returns>
        public bool SetLastFiredCommand(object? sender, ICommand command)
        {
            if (sender == null || sender != lastFiredCommandSender || LastFiredCommand != command)
            {
                LastFiredCommand = command;
                lastFiredCommandSender = sender;
            }
            else
            {
                LastFiredCommand = null;
                lastFiredCommandSender = null;
            }

            return LastFiredCommand != null;
        }

        #endregion

    }
}
