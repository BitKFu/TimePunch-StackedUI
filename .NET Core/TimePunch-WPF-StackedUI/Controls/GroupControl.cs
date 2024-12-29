using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace TimePunch.StackedUI.Controls
{
    [ContentProperty(nameof(Example))]
    public class GroupControl : Control
    {
        static GroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupControl), new FrameworkPropertyMetadata(typeof(GroupControl)));
        }

        #region HeaderText

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                nameof(HeaderText),
                typeof(string),
                typeof(GroupControl),
                new PropertyMetadata(string.Empty));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        #endregion

        #region Example

        public static readonly DependencyProperty ExampleProperty =
            DependencyProperty.Register(
                nameof(Example),
                typeof(object),
                typeof(GroupControl),
                new PropertyMetadata());

        public object Example
        {
            get => GetValue(ExampleProperty);
            set => SetValue(ExampleProperty, value);
        }

        #endregion

        #region Options

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(
                nameof(Options),
                typeof(object),
                typeof(GroupControl),
                null);

        public object Options
        {
            get => GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }

        #endregion

        #region MaxContentWidth

        public static readonly DependencyProperty MaxContentWidthProperty =
            DependencyProperty.Register(
                nameof(MaxContentWidth),
                typeof(double),
                typeof(GroupControl),
                new PropertyMetadata(1028d));

        public double MaxContentWidth
        {
            get => (double)GetValue(MaxContentWidthProperty);
            set => SetValue(MaxContentWidthProperty, value);
        }

        #endregion
    }
}
