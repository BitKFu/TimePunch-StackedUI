using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace TimePunch.StackedUI.Controls
{
    public partial class PageControl : ContentControl
    {
        #region Property MenuProperty

        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.RegisterAttached("Menu", typeof(MenuBar), typeof(PageControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the CanGoBack.
        /// </summary>
        /// <value>The CanGoBack.</value>
        public MenuBar Menu
        {
            get => (MenuBar)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        #endregion
        
        #region Property TitleProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(string), typeof(PageControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the CanGoBack.
        /// </summary>
        /// <value>The CanGoBack.</value>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion


    }
}
