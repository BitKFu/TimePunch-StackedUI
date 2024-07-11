using System.Collections.ObjectModel;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Model;

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
            MenuItems = new ObservableCollection<MenuItemModel>();
        }

        #region Property MenuItems

        /// <summary>
        /// Gets or sets the $Property$.
        /// </summary>
        /// <value>The $Property$.</value>
        public ObservableCollection<MenuItemModel> MenuItems
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
    }
}
