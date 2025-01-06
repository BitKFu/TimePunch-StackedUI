using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimePunch.StackedUI.Controls
{
    public sealed class DpiDecorator : ContentControl
    {
        private readonly ScaleTransform dpiTransform;

        public DpiDecorator()
        {
            DefaultStyleKey = typeof(DpiDecorator);
            dpiTransform = new ScaleTransform(){ScaleX = Scale, ScaleY = Scale, CenterX = 0, CenterY = 0};
            SizeChanged += DpiDecorator_SizeChanged;
        }

        private void DpiDecorator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var child = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            if (child == null)
                return;

            var content = (ContentControl)sender;
            content.Width = double.NaN;
            content.Height = double.NaN;

            child.Width = e.NewSize.Width / Scale;
            child.Height = e.NewSize.Height / Scale;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var child = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            if (child == null)
                return;

            var uiContent = child.Content as UIElement;
            if (uiContent == null)
                return;
            uiContent.RenderTransform = dpiTransform;
            uiContent.RenderTransformOrigin = new Point(0, 0);
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

        public new double Scale
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

            // Force a resize event to update the size of the child
            dpiDecorator.Width = dpiDecorator.ActualWidth-1;
        }
    }
}
