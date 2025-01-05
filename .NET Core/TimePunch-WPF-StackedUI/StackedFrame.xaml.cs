using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Extensions;
using TimePunch.StackedUI.Model;

namespace TimePunch.StackedUI
{
    /// <summary>
    /// Interaction logic for StackedFrame.xaml
    /// </summary>
    public partial class StackedFrame : UserControl
    {
        private readonly Stack<Frame> frameStack = new Stack<Frame>();
        private readonly Dictionary<Frame, GridSplitter> splitters = new Dictionary<Frame, GridSplitter>();

        private readonly object fadeInOut = new object();

        public StackedFrame()
        {
            InitializeComponent();
        }

        #region Property CanGoBack

        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.RegisterAttached("CanGoBack", typeof(bool), typeof(StackedFrame), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the CanGoBack.
        /// </summary>
        /// <value>The CanGoBack.</value>
        public bool CanGoBack
        {
            get => (bool)GetValue(CanGoBackProperty) && !Monitor.IsEntered(fadeInOut);
            set => SetValue(CanGoBackProperty, value);
        }

        #endregion

        #region Property TopFrame

        public static readonly DependencyProperty TopFrameProperty =
            DependencyProperty.RegisterAttached("TopFrame", typeof(Frame), typeof(StackedFrame), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TopFrame.
        /// </summary>
        /// <value>The TopFrame.</value>
        public Frame? TopFrame
        {
            get => (Frame)GetValue(TopFrameProperty);
            set => SetValue(TopFrameProperty, value);
        }

        #endregion

        #region Property Duration

        public static readonly DependencyProperty FadeInDurationProperty =
            DependencyProperty.RegisterAttached("FadeInDuration", typeof(int), typeof(StackedFrame), new PropertyMetadata(200));

        public int FadeInDuration
        {
            get => (int)GetValue(FadeInDurationProperty);
            set => SetValue(FadeInDurationProperty, value);
        }

        #endregion

        #region Property Duration

        public static readonly DependencyProperty FadeOutDurationProperty =
            DependencyProperty.RegisterAttached("FadeOutDuration", typeof(int), typeof(StackedFrame), new PropertyMetadata(50));

        public int FadeOutDuration
        {
            get => (int)GetValue(FadeOutDurationProperty);
            set => SetValue(FadeOutDurationProperty, value);
        }

        #endregion

        private Task FadeIn(UIElement element)
        {
            var sb = new Storyboard();
            var duration = TimeSpan.FromMilliseconds(FadeInDuration);
            var animation = new DoubleAnimation(0.0, 1.0, duration);
            animation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(animation);

            var tcs = new TaskCompletionSource<bool>();
            void OnComplete(object sender, EventArgs e)
            {
                sb.Completed -= OnComplete;
                element.Opacity = 1;
                tcs.SetResult(true);
            }

            sb.Completed += OnComplete;
            sb.Begin();
            return tcs.Task;
        }

        private Task FadeOut(UIElement element)
        {
            var sb = new Storyboard();
            var duration = TimeSpan.FromMilliseconds(FadeOutDuration);
            var animation = new DoubleAnimation(1.0, 0.0, duration);
            animation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(animation);

            var tcs = new TaskCompletionSource<bool>();
            void OnComplete(object sender, EventArgs e)
            {
                sb.Completed -= OnComplete;
                element.Opacity = 0;
                tcs.SetResult(true);
            }

            sb.Completed += OnComplete;
            sb.Begin();
            return tcs.Task;
        }

        /// <summary>
        /// This method sets only the last column definition to star width
        /// </summary>
        private void AdjustColumnWidths(double pageWidth)
        {
            if (StackedMode == StackedMode.InPlace)
            {
                // Hide all previous columns
                for (int i = 0; i < StackGrid.ColumnDefinitions.Count; i++)
                {
                    var last = i == StackGrid.ColumnDefinitions.Count - 1;
                    if (!last)
                    {
                        StackGrid.ColumnDefinitions[i].MinWidth = 0;
                        StackGrid.ColumnDefinitions[i].Width = new GridLength(0);
                    }
                    else
                    {
                        StackGrid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                    }
                }
            }
            else
            {
                // Adjust the with of the previous columns
                for (int i = 0; i < StackGrid.ColumnDefinitions.Count; i++)
                {
                    var last = i >= StackGrid.ColumnDefinitions.Count - 2;

                    if (!last && i % 2 == 0)
                    {
                        // Minimize all prior pages
                        StackGrid.ColumnDefinitions[i].Width = StackGrid.ColumnDefinitions[i].MinWidth > 0
                            ? new GridLength(StackGrid.ColumnDefinitions[i].MinWidth)
                            : StackGrid.ColumnDefinitions[i].Width;
                    }
                    else
                    {
                        if (last && i % 2 == 0)
                            StackGrid.ColumnDefinitions[i].Width = double.IsNaN(pageWidth)
                                ? new GridLength(1, GridUnitType.Star)
                                : new GridLength(pageWidth);
                    }
                }

                if (StackGrid.ColumnDefinitions.Count > 2)
                    ScrollViewer.ScrollToRightEnd();
            }
        }

        public async Task AddFrame(IEventAggregator eventAggregator, Frame frame, Page page)
        {
            Monitor.TryEnter(fadeInOut, -1);

            try
            {
                // add a new column
                var column = StackGrid.ColumnDefinitions.Count - (StackedMode == StackedMode.Resizeable ? 1 : 0);
                var columnDefinition = new ColumnDefinition()
                {
                    MinWidth = page.MinWidth,
                    MaxWidth = page.MaxWidth
                };

                var pageWidth = page.Width;

                StackGrid.ColumnDefinitions.Insert(column, columnDefinition);
                Grid.SetColumn(frame, column);
                page.Width = double.NaN;

                // Update max with of page - if it's an inplace update
                if (StackedMode == StackedMode.InPlace)
                    page.MaxWidth = double.PositiveInfinity;

                // Check if the page has an adorner decorator
                if (!(page.Content is AdornerDecorator))
                {
                    var content = page.Content as UIElement;
                    page.Content = null;
                    var adornerDecorator = new AdornerDecorator() { Child = content };
                    page.Content = adornerDecorator;
                }

                // push the frame to the new column
                frame.Opacity = FadeInDuration > 0 ? 0 : 1;
                frame.Content = page;
                frameStack.Push(frame);
                StackGrid.Children.Add(frame);

                AdjustColumnWidths(pageWidth);
                UpdateTopFrame();
                BreadCrumbs.Add(new BreadCrumbNavigation(eventAggregator, page));

                if (FadeInDuration > 0)
                    await FadeIn(frame);
            }
            finally
            {
                Monitor.Exit(fadeInOut);
            }
        }

        public void AddSplitter(Frame frame)
        {
            lock (splitters)
            {
                // check if there is already a splitter
                if (splitters.ContainsKey(frame))
                    return;

                var splitter = new GridSplitter
                {
                    ShowsPreview = false,
                    ResizeDirection = GridResizeDirection.Columns,
                    ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 0, 20),
                };

                // add the splitter 
                var column = StackGrid.ColumnDefinitions.Count - (StackedMode == StackedMode.Resizeable ? 1 : 0);
                StackGrid.ColumnDefinitions.Insert(column, new ColumnDefinition() { Width = new GridLength(SplitterWidth) });
                Grid.SetColumn(splitter, column);

                StackGrid.Children.Add(splitter);

                // push the splitter to the frame dictionary
                splitters.Add(frame, splitter);
            }
        }

        internal async Task GoBack(bool animate)
        {
            if (frameStack.Count == 0)
                return;

            Monitor.TryEnter(fadeInOut, -1);

            try
            {
                // remove the frame
                var removedFrame = frameStack.Pop(); // get the frame to remove
                if (animate && FadeOutDuration > 0)
                    await FadeOut(removedFrame);

                var column = Grid.GetColumn(removedFrame); // get the column in grid to remove
                StackGrid.Children.Remove(removedFrame); // remove the frame
                StackGrid.ColumnDefinitions.RemoveAt(column); // remove the columne

                // remove the splitter if there is one
                var removedSplitter = StackedMode == StackedMode.Resizeable ? removedFrame : frameStack.Any() ? frameStack.Peek() : null;
                if (removedSplitter != null && splitters.ContainsKey(removedSplitter))
                {
                    var splitter = splitters[removedSplitter]; // get the splitter to remove
                    column = Grid.GetColumn(splitter); // get the column in grid to remove
                    StackGrid.Children.Remove(splitter); // remove the splitter
                    StackGrid.ColumnDefinitions.RemoveAt(column); // remove the columne

                    splitters.Remove(removedSplitter); // remove the splitter / frame binding
                }
                // That might be a special case, when switching from InPlace to Resizeable
                else if (StackedMode == StackedMode.Resizeable && splitters.Count == 1)
                {
                    var splitter = splitters.First().Value;

                    column = Grid.GetColumn(splitter); // get the column in grid to remove
                    StackGrid.Children.Remove(splitter); // remove the splitter
                    StackGrid.ColumnDefinitions.RemoveAt(column); // remove the columne

                    splitters.Clear();
                }

                if (removedFrame.Content is Page { DataContext: IDisposable vmPageDataContext })
                {
                    // Dispose the data model
                    vmPageDataContext.Dispose();
                }

                UpdateTopFrame();

                if (TopFrame?.Content is Page page)
                    AdjustColumnWidths(page.Width);

                lock (BreadCrumbs)
                {
                    if (BreadCrumbs.Count > 0)
                        BreadCrumbs.RemoveAt(BreadCrumbs.Count - 1);
                }
            }
            finally
            {
                Monitor.Exit(fadeInOut);
            }
        }

        private void UpdateTopFrame()
        {
            CanGoBack = frameStack.Count() > 1;
            TopFrame = frameStack.Any() ? frameStack.Peek() : null;
        }

        public void DisableTop()
        {
            var topFrame = frameStack.Count > 0 ? frameStack.Peek() : null;
            if (topFrame != null)
                topFrame.IsEnabled = false;
        }

        public void EnableTop()
        {
            if (frameStack.Count == 0)
                return;

            var topFrame = frameStack.Peek();
            topFrame.IsEnabled = true;
        }

        public bool Contains(string frameKey)
        {
            foreach (var frame in frameStack)
            {
                var page = frame.Content as Page;
                if (page == null)
                    continue;

                var key = StackedFrameExtension.GetFrameKey(page);
                if (key == frameKey)
                    return true;
            }

            return false;
        }


        #region Property BreadCrumbs

        public static readonly DependencyProperty BreadCrumbsProperty =
            DependencyProperty.RegisterAttached("BreadCrumbs", typeof(ObservableCollection<BreadCrumbNavigation>), typeof(StackedFrame), new PropertyMetadata(new ObservableCollection<BreadCrumbNavigation>()));

        /// <summary>
        /// Gets or sets the BreadCrumbs.
        /// </summary>
        /// <value>The BreadCrumbs.</value>
        public ObservableCollection<BreadCrumbNavigation> BreadCrumbs
        {
            get => (ObservableCollection<BreadCrumbNavigation>)GetValue(BreadCrumbsProperty);
            set => SetValue(BreadCrumbsProperty, value);
        }

        #endregion

        /// <summary>
        /// Initializes the stacked frame by using the stacked mode
        /// </summary>
        public void Initialize()
        {
            // add an empty column
            if (StackedMode == StackedMode.Resizeable)
                StackGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        #region Property StackedMode

        public static readonly DependencyProperty StackedModeProperty =
            DependencyProperty.RegisterAttached("StackedMode", typeof(StackedMode), typeof(StackedFrame), new PropertyMetadata(StackedMode.Resizeable));

        /// <summary>
        /// Gets or sets the stacked mode
        /// </summary>
        public StackedMode StackedMode
        {
            get => (StackedMode)GetValue(StackedModeProperty);
            set => SetValue(StackedModeProperty, value);
        }

        #endregion

        #region Property SplitterWith

        public static readonly DependencyProperty SplitterWidthProperty =
            DependencyProperty.RegisterAttached("SplitterWidth", typeof(int), typeof(StackedFrame), new PropertyMetadata(3));

        /// <summary>
        /// Gets or sets the with of the splitter
        /// </summary>
        public int SplitterWidth
        {
            get => (int)GetValue(SplitterWidthProperty);
            set => SetValue(SplitterWidthProperty, value);
        }

        #endregion

        #region Property PropertyPanelVisibility

        public static readonly DependencyProperty PropertyPanelVisibilityProperty =
            DependencyProperty.RegisterAttached("PropertyPanelVisibility", typeof(Visibility), typeof(StackedFrame), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the with of the splitter
        /// </summary>
        public Visibility PropertyPanelVisibility
        {
            get => (Visibility)GetValue(PropertyPanelVisibilityProperty);
            set
            {
                // Maybe we don't need to change
                if (PropertyPanelVisibility == value)
                    return;

                UpdatePropertyVisibility(value);
            }
        }

        private async void UpdatePropertyVisibility(Visibility value)
        {
            Monitor.TryEnter(fadeInOut, -1);

            try
            {
                // Change the visibility
                if (value == Visibility.Visible)
                {
                    SetValue(PropertyPanelVisibilityProperty, value);
                    if (FadeInDuration > 0)
                        await FadeIn(PropertyPanel);
                }
                else
                {
                    if (FadeOutDuration > 0)
                        await FadeOut(PropertyPanel);

                    SetValue(PropertyPanelVisibilityProperty, value);
                }
            }
            finally
            {
                Monitor.Exit(fadeInOut);
            }
        }

        #endregion
    }
}
