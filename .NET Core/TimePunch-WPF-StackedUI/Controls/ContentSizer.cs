using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TimePunch.StackedUI.Controls
{
    [TemplatePart(Name = ThumbPartName, Type = typeof(Thumb))]
    public class ContentSizer : Control
    {
        private const string ThumbPartName = "PART_Thumb";
        private Thumb _thumb;

        static ContentSizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ContentSizer),
                new FrameworkPropertyMetadata(typeof(ContentSizer)));
        }

        public ContentSizer()
        {
            Focusable = true;
        }

        #region Dependency Properties

        public static readonly DependencyProperty TargetControlProperty =
            DependencyProperty.Register(
                nameof(TargetControl),
                typeof(FrameworkElement),
                typeof(ContentSizer),
                new PropertyMetadata(null));

        /// <summary>
        /// Optional: Element, dessen Größe geändert wird.
        /// Wenn null, wird der visuelle Parent verwendet.
        /// </summary>
        public FrameworkElement TargetControl
        {
            get => (FrameworkElement)GetValue(TargetControlProperty);
            set => SetValue(TargetControlProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(ContentSizer),
                new PropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// Vertical = vertikaler Balken, ändert Width (HorizontalChange).
        /// Horizontal = horizontaler Balken, ändert Height (VerticalChange).
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty IsDragInvertedProperty =
            DependencyProperty.Register(
                nameof(IsDragInverted),
                typeof(bool),
                typeof(ContentSizer),
                new PropertyMetadata(false));

        /// <summary>
        /// Invertiert die Drag-Richtung.
        /// </summary>
        public bool IsDragInverted
        {
            get => (bool)GetValue(IsDragInvertedProperty);
            set => SetValue(IsDragInvertedProperty, value);
        }

        public static readonly DependencyProperty DragIncrementProperty =
            DependencyProperty.Register(
                nameof(DragIncrement),
                typeof(double),
                typeof(ContentSizer),
                new PropertyMetadata(1.0));

        /// <summary>
        /// Schrittweite für Maus/Toucheingaben (Snap).
        /// </summary>
        public double DragIncrement
        {
            get => (double)GetValue(DragIncrementProperty);
            set => SetValue(DragIncrementProperty, value);
        }

        public static readonly DependencyProperty KeyboardIncrementProperty =
            DependencyProperty.Register(
                nameof(KeyboardIncrement),
                typeof(double),
                typeof(ContentSizer),
                new PropertyMetadata(8.0));

        /// <summary>
        /// Schrittweite pro Pfeiltaste.
        /// </summary>
        public double KeyboardIncrement
        {
            get => (double)GetValue(KeyboardIncrementProperty);
            set => SetValue(KeyboardIncrementProperty, value);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_thumb != null)
            {
                _thumb.DragDelta -= OnThumbDragDelta;
            }

            _thumb = GetTemplateChild(ThumbPartName) as Thumb;

            if (_thumb != null)
            {
                _thumb.DragDelta += OnThumbDragDelta;
            }

            // Standard-Cursor setzen, falls nicht explizit gesetzt
            if (Cursor == null)
            {
                Cursor = Orientation == Orientation.Vertical
                    ? Cursors.SizeWE
                    : Cursors.SizeNS;
            }
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var target = TargetControl ?? Parent as FrameworkElement;
            if (target == null)
            {
                return;
            }

            double delta;

            if (Orientation == Orientation.Vertical)
            {
                // Vertikaler Balken -> Breite ändern
                delta = e.HorizontalChange;
            }
            else
            {
                // Horizontaler Balken -> Höhe ändern
                delta = e.VerticalChange;
            }

            if (IsDragInverted)
            {
                delta = -delta;
            }

            // Auf Schrittweite runden
            var increment = Math.Max(DragIncrement, 0.1);
            delta = Math.Round(delta / increment) * increment;

            if (Orientation == Orientation.Vertical)
            {
                ResizeWidth(target, delta);
            }
            else
            {
                ResizeHeight(target, delta);
            }
        }

        private static void ResizeWidth(FrameworkElement target, double delta)
        {
            double current = double.IsNaN(target.Width)
                ? target.ActualWidth
                : target.Width;

            double newWidth = current + delta;
            newWidth = Math.Max(target.MinWidth, newWidth);
            if (!double.IsNaN(target.MaxWidth) && !double.IsPositiveInfinity(target.MaxWidth))
            {
                newWidth = Math.Min(target.MaxWidth, newWidth);
            }

            target.Width = newWidth;
        }

        private static void ResizeHeight(FrameworkElement target, double delta)
        {
            double current = double.IsNaN(target.Height)
                ? target.ActualHeight
                : target.Height;

            double newHeight = current + delta;
            newHeight = Math.Max(target.MinHeight, newHeight);
            if (!double.IsNaN(target.MaxHeight) && !double.IsPositiveInfinity(target.MaxHeight))
            {
                newHeight = Math.Min(target.MaxHeight, newHeight);
            }

            target.Height = newHeight;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var target = TargetControl ?? Parent as FrameworkElement;
            if (target == null)
            {
                return;
            }

            double step = KeyboardIncrement;
            double delta = 0;

            switch (e.Key)
            {
                case Key.Left:
                    if (Orientation == Orientation.Vertical)
                        delta = -step;
                    break;
                case Key.Right:
                    if (Orientation == Orientation.Vertical)
                        delta = step;
                    break;
                case Key.Up:
                    if (Orientation == Orientation.Horizontal)
                        delta = -step;
                    break;
                case Key.Down:
                    if (Orientation == Orientation.Horizontal)
                        delta = step;
                    break;
                default:
                    return;
            }

            if (IsDragInverted)
            {
                delta = -delta;
            }

            if (Orientation == Orientation.Vertical)
            {
                ResizeWidth(target, delta);
            }
            else
            {
                ResizeHeight(target, delta);
            }

            e.Handled = true;
        }
    }
}
