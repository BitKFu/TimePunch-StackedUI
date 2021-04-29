using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TimePunch.StackedUI.Extensions
{
    public static class StackedFrameExtension
    {
        #region Frame Key

        public static readonly DependencyProperty FrameKeyProperty =
            DependencyProperty.RegisterAttached("FrameKey", typeof(string), typeof(Page), new PropertyMetadata(string.Empty));

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
