using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Controls;
using TimePunch.StackedUI.Extensions;
using TimePunch.StackedUI.Model;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace TimePunch.StackedUI
{
    /// <summary>
    /// Interaction logic for StackedFrame.xaml
    /// </summary>
    public partial class StackedFrame : UserControl
    {
        private readonly Stack<Frame> frameStack = new();
        private readonly Dictionary<Frame, ContentSizer> splitters = new();

        private readonly object fadeInOut = new object();

        public StackedFrame()
        {
            InitializeComponent();
            SizeChanged += StackedFrame_SizeChanged;
        }

        public virtual void Initialize()
        {
        }

        private void StackedFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsNextToTopFrame && TopFrame != null && StackedMode == StackedMode.Resizeable)
                AdjustColumnWidths(TopFrame?.Width ?? Double.NaN);
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
                for (int i = 0; i < StackPanel.Children.Count; i++)
                {
                    var element = (FrameworkElement)StackPanel.Children[i];

                    var last = i == StackPanel.Children.Count - 1;
                    if (!last)
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        element.Width = Math.Min(Frame.ActualWidth, StackPanel.ActualWidth);
                        element.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                // Adjust the with of the previous columns
                for (int i = 0; i < StackPanel.Children.Count; i++)
                {
                    var element = (FrameworkElement)StackPanel.Children[i];
                    var last = i >= StackPanel.Children.Count - 2;

                    if (!last && i % 2 == 0)
                    {
                        // Minimize all prior pages
                        element.Width = element.MinWidth > 0
                            ? element.MinWidth
                            : element.Width;
                    }
                    else
                    {
                        if (last && i % 2 == 0)
                        {
                            var checkSize
                                = IsNextToTopFrame
                                    ? ((ScrollViewer)((StackPanel)element.Parent).Parent).ActualWidth
                                    : pageWidth;

                            while (Math.Abs(element.Width - checkSize) > 1)
                                element.Width = checkSize;
                        }
                    }
                }
            }
        }

        public async Task AddFrame(IEventAggregator eventAggregator, Frame frame, Page page)
        {
            Monitor.TryEnter(fadeInOut, -1);

            try
            {
                // add a new column
                frame.Width = double.IsNaN(page.Width)
                    ? page.MinWidth
                    : page.Width;
                frame.MinWidth = page.MinWidth;
                frame.MaxWidth = page.MaxWidth;
                frame.HorizontalContentAlignment = HorizontalAlignment.Stretch;

                // Update max with of page - if it's an inplace update
                if (StackedMode == StackedMode.InPlace)
                    page.MaxWidth = double.PositiveInfinity;

                var topFrameMinWith = TopFrame?.MinWidth ?? 0;

                // push the frame to the new column
                frame.Opacity = FadeInDuration > 0 ? 0 : 1;
                frame.Content = page;
                frameStack.Push(frame);
                StackPanel.Children.Add(frame);

                var pageWidth = double.IsNaN(page.Width)
                    ? Math.Max(page.MinWidth, ScrollViewer.ActualWidth - 12 - topFrameMinWith) : page.Width;

                AdjustColumnWidths(pageWidth);
                page.Width = double.NaN; // Make page resizeable (otherwise content gets centered)

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

                // add the splitter 
                var splitter = new ContentSizer()
                {
                    TargetControl = (FrameworkElement)StackPanel.Children[StackPanel.Children.Count - 1],
                };

                // push the splitter to the frame dictionary
                splitters.Add(frame, splitter);

                // At last, add the splitter to the stack panel
                StackPanel.Children.Add(splitter);
            }
        }

        public async Task GoBack(bool animate)
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

                StackPanel.Children.Remove(removedFrame); // remove the frame

                // remove the splitter if there is one
                var removedSplitter = StackedMode == StackedMode.Resizeable ? removedFrame : frameStack.Any() ? frameStack.Peek() : null;
                if (removedSplitter != null && splitters.ContainsKey(removedSplitter))
                {
                    var splitter = splitters[removedSplitter]; // get the splitter to remove
                    StackPanel.Children.Remove(splitter); // remove the splitter
                    splitters.Remove(removedSplitter); // remove the splitter / frame binding

                    // remove the last splitter, because the first one has none
                    if (splitters.Count == 1)
                    {
                        splitter = splitters.First().Value; // get the splitter to remove
                        StackPanel.Children.Remove(splitter); // remove the splitter
                        splitters.Remove(removedSplitter); // remove the splitter / frame binding

                        splitters.Clear();
                    }
                }
                // That might be a special case, when switching from InPlace to Resizeable
                else if (StackedMode == StackedMode.Resizeable && splitters.Count == 1)
                {
                    var splitter = splitters.First().Value;
                    StackPanel.Children.Remove(splitter);
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

        public bool IsTopFrame => !frameStack.Any();

        public bool IsNextToTopFrame => frameStack.Count() == 1;

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

        public void UpdateScrollPosition()
        {
            // we only need to scroll to the end, if the page does not contain a grid!
            var scroll = VisualTreeHelperEx.FindDescendantByType<ScrollViewer>(this);
            if (scroll == null)
                return;

            // Get the key of the new top page - to set the with
            if (TopFrame?.Parent is StackPanel panel)
            {
                var currentColumn = panel.Children.IndexOf(TopFrame);
                if (currentColumn > 1)
                {
                    scroll.ScrollToHorizontalOffset(double.PositiveInfinity);
                }
                else
                {
                    // Top Page always starts at position 0
                    scroll.ScrollToHorizontalOffset(0);
                }
            }
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

                UpdatePropertyVisibilityNonAsync(value);
            }
        }

        private async void UpdatePropertyVisibilityNonAsync(Visibility value)
        {
            await UpdatePropertyVisibility(value);

            if (TopFrame != null && StackedMode == StackedMode.Resizeable)
            {
                await Task.Delay(50).ContinueWith(t => Dispatcher.Invoke(() =>
                {
                    AdjustColumnWidths(TopFrame?.Width ?? Double.NaN);
                    UpdateScrollPosition();
                }));
            }
        }

        private async Task UpdatePropertyVisibility(Visibility value)
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
