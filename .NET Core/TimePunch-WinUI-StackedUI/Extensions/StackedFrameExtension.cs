using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimePunch.StackedUI.Extensions
{
    public static class StackedFrameExtension
    {
        #region Frame Key

        public static readonly DependencyProperty FrameKeyProperty =
            DependencyProperty.RegisterAttached("FrameKey", typeof(string), typeof(Page), new PropertyMetadata(Guid.NewGuid().ToString()));

        public static void SetFrameKey(DependencyObject dp, string value)
        {
            dp?.SetValue(FrameKeyProperty, value);
        }

        public static string GetFrameKey(DependencyObject dp)
        {
            return (string)dp.GetValue(FrameKeyProperty);
        }

        #endregion

    }
}
