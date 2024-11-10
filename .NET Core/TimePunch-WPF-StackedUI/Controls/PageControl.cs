using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace TimePunch.StackedUI.Controls
{
    [ContentProperty(nameof(Content))]
    public class PageControl : Control
    {
        #region Property CanGoBack

        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.RegisterAttached("Menu", typeof(Menu), typeof(PageControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the CanGoBack.
        /// </summary>
        /// <value>The CanGoBack.</value>
        public Menu Menu
        {
            get => (Menu)GetValue(MenuProperty);
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

        #region Content

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(PageControl),
                null);

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

    }
}
