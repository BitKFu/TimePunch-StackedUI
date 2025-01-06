using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TimePunch.StackedUI.Controls
{
    public class DpiDecorator : Decorator
    {
        private readonly ScaleTransform dpiTransform;

        public DpiDecorator()
        {
            dpiTransform = new ScaleTransform(Scale, Scale);

            Loaded += (_, _) =>
            {
                LayoutTransform = dpiTransform;
            };
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                nameof(Scale),
                typeof(double),
                typeof(DpiDecorator),
                new PropertyMetadata(1.0, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetScale(d as DpiDecorator, (double)e.NewValue);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static void SetScale(DpiDecorator? dpiDecorator, double scale)
        {
            if (dpiDecorator == null)
                return;

            dpiDecorator.dpiTransform.ScaleX = scale;
            dpiDecorator.dpiTransform.ScaleY = scale;
        }
    };
}
