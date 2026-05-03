using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimePunch.StackedUI.Controls
{
    public class AttachedProperties
    {
        #region DialogResult

        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(AttachedProperties), new PropertyMetadata(default(bool?), OnDialogResultChanged));

        private static void OnDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is Microsoft.UI.Xaml.Window wnd)
            //{
            //    if (wnd.DialogResult == null)
            //        wnd.DialogResult = (bool?) e.NewValue;
            //}

            if (d is ModalDialog mod)
            {
                if (mod.DialogResult == null)
                    mod.DialogResult = (bool?) e.NewValue;
            }
        }

        public static bool? GetDialogResult(DependencyObject dp)
        {
            if (dp == null) throw new ArgumentNullException(nameof(dp));

            return (bool?)dp.GetValue(DialogResultProperty);
        }

        public static void SetDialogResult(DependencyObject dp, bool? value)
        {
            if (dp == null) throw new ArgumentNullException(nameof(dp));

            dp.SetValue(DialogResultProperty, value);
        }

        #endregion

        #region Focus

        public static readonly DependencyProperty TextFocusProperty =
            DependencyProperty.RegisterAttached("TextFocus", typeof (bool), typeof (AttachedProperties),
                new PropertyMetadata(false, OnTextFocusChanged));

        private static void OnTextFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tbb = d as TextBox;
            if (tbb == null)
                return;

            if ((bool)e.NewValue)
                tbb.GotFocus += OnFocusTextContent;
            else
                tbb.GotFocus -= OnFocusTextContent;
        }

        private static void OnFocusTextContent(object sender, RoutedEventArgs e)
        {
            var tbb = sender as TextBox;
            tbb?.SelectAll();
        }

        public static bool GetTextFocus(DependencyObject dp)
        {
            if (dp == null) throw new ArgumentNullException(nameof(dp));
            return (bool) dp.GetValue(TextFocusProperty);
        }

        public static void SetTextFocus(DependencyObject dp, object value)
        {
            if (dp == null) throw new ArgumentNullException(nameof(dp));

            if (value is string s && bool.TryParse(s, out var valueBool))
                dp.SetValue(TextFocusProperty, valueBool);
            else 
                dp.SetValue(TextFocusProperty, value);
        }

        #endregion
    }
}
