using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TimePunch.StackedUI.Model
{
    public class MenuItemModel : DependencyObject
    {
        #region Property Header

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached("Header", typeof(string), typeof(MenuItemModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the Header.
        /// </summary>
        /// <value>The Header.</value>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region Property Command

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MenuItemModel), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Command.
        /// </summary>
        /// <value>The Command.</value>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion

        #region Property InputGestureText

        public static readonly DependencyProperty InputGestureTextProperty =
            DependencyProperty.RegisterAttached("InputGestureText", typeof(string), typeof(MenuItemModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the InputGestureText.
        /// </summary>
        /// <value>The InputGestureText.</value>
        public string InputGestureText
        {
            get => (string)GetValue(InputGestureTextProperty);
            set => SetValue(InputGestureTextProperty, value);
        }

        #endregion

        #region Property MenuItems

        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.RegisterAttached("MenuItems", typeof(ObservableCollection<MenuItemModel>), typeof(MenuItemModel), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the MenuItems.
        /// </summary>
        /// <value>The MenuItems.</value>
        public ObservableCollection<MenuItemModel> MenuItems
        {
            get => (ObservableCollection<MenuItemModel>)GetValue(MenuItemsProperty);
            set => SetValue(MenuItemsProperty, value);
        }

        #endregion
    }
}
