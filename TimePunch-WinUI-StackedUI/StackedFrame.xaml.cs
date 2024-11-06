using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.Controls;
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
            get => (bool)GetValue(CanGoBackProperty);
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
            var animation = new DoubleAnimation() { From = 0.0, To = 1.0, Duration = duration };
            animation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, "Opacity");
            sb.Children.Add(animation);

            var tcs = new TaskCompletionSource<bool>();
            void OnComplete(object? sender, object e)
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
            var animation = new DoubleAnimation() { From = 1.0, To = 0.0, Duration = duration };
            animation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, "Opacity");
            sb.Children.Add(animation);

            var tcs = new TaskCompletionSource<bool>();
            void OnComplete(object? sender, object e)
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
        private void AdjustColumnWidths()
        {
            if (StackedMode == StackedMode.InPlace)
            {
                // Hide all previous columns
                for (int i = 0; i < StackPanel.ColumnDefinitions.Count; i++)
                {
                    var last = i == StackPanel.ColumnDefinitions.Count - 1;
                    if (!last)
                    {
                        StackPanel.ColumnDefinitions[i].MinWidth = 0;
                        StackPanel.ColumnDefinitions[i].Width = new GridLength(0);
                    }
                    else
                    {
                        StackPanel.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                    }
                }
            }
            else
            {
                // Adjust the with of the previous columns
                for (int i = 0; i < StackPanel.ColumnDefinitions.Count; i++)
                {
                    var last = i == StackPanel.ColumnDefinitions.Count - 1;

                    if (!last && i % 2 == 0)
                    {
                        // Minimize all prior pages
                        StackPanel.ColumnDefinitions[i].Width = StackPanel.ColumnDefinitions[i].MinWidth > 0
                            ? new GridLength(StackPanel.ColumnDefinitions[i].MinWidth)
                            : GridLength.Auto;
                    }
                    else
                    {
                        if (last && i % 2 == 0)
                            StackPanel.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                    }
                }
            }
        }

        public async Task AddFrame(IEventAggregator eventAggregator, Frame frame, Page page)
        {
            // add a new column
            var column = StackPanel.ColumnDefinitions.Count - (StackedMode == StackedMode.Resizeable ? 1 : 0);
            StackPanel.ColumnDefinitions.Insert(column,
                new ColumnDefinition()
                {
                    Width = double.IsNaN(page.Width) || StackedMode == StackedMode.FullWidth
                        ? GridLength.Auto
                        : new GridLength(page.Width),
                    MinWidth = page.MinWidth,
                    MaxWidth = StackedMode == StackedMode.InPlace 
                        ? double.PositiveInfinity
                        : page.MaxWidth
                });
            Grid.SetColumn(frame, column);
            page.Width = double.NaN;

            // Update max with of page - if it's an inplace update
            if (StackedMode == StackedMode.InPlace)
                page.MaxWidth = double.PositiveInfinity;

            // Check if the page has an adorner decorator
            // @todo: Find a replacement for an AdornerDecorator

            //if (!(page.Content is AdornerDecorator))
            //{
            //    var content = page.Content as UIElement;
            //    page.Content = null;
            //    var adornerDecorator = new AdornerDecorator() { Child = content };
            //    page.Content = adornerDecorator;
            //}

            // push the frame to the new column
            frame.Opacity = FadeInDuration > 0 ? 0 : 1;
            frame.Content = page;
            frameStack.Push(frame);
            StackPanel.Children.Add(frame);

            AdjustColumnWidths();
            UpdateTopFrame();
            BreadCrumbs.Add(new BreadCrumbNavigation(eventAggregator, page));

            if (FadeInDuration > 0)
                await FadeIn(frame);
        }

        public void AddSplitter()
        {
            var splitter = new GridSplitter
            {
                ResizeDirection = GridSplitter.GridResizeDirection.Columns,
                ResizeBehavior = GridSplitter.GridResizeBehavior.BasedOnAlignment,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            
            // add the splitter 
            var column = StackPanel.ColumnDefinitions.Count - (StackedMode == StackedMode.Resizeable ? 1 : 0);
            StackPanel.ColumnDefinitions.Insert(column, new ColumnDefinition() { Width = SplitterWidth });
            Grid.SetColumn(splitter, column);

            StackPanel.Children.Add(splitter);

            // push the splitter to the frame dictionary
            splitters.Add(frameStack.Peek(), splitter);
        }

        public async Task GoBack(bool animate)
        {
            if (frameStack.Count == 0)
            {
                //UpdateTopFrame();
                return;
            }

            // remove the frame
            var removedFrame = frameStack.Pop(); // get the frame to remove
            if (animate && FadeOutDuration>0)
                await FadeOut(removedFrame);

            var column = Grid.GetColumn(removedFrame); // get the column in grid to remove
            StackPanel.Children.Remove(removedFrame); // remove the frame
            StackPanel.ColumnDefinitions.RemoveAt(column); // remove the columne

            // remove the splitter if there is one
            var removedSplitter = StackedMode == StackedMode.Resizeable ? removedFrame : frameStack.Any() ? frameStack.Peek() : null;
            if (removedSplitter != null && splitters.ContainsKey(removedSplitter))
            {
                var splitter = splitters[removedSplitter]; // get the splitter to remove
                column = Grid.GetColumn(splitter); // get the column in grid to remove
                StackPanel.Children.Remove(splitter); // remove the splitter
                splitters.Remove(removedSplitter); // remove the splitter / frame binding
                StackPanel.ColumnDefinitions.RemoveAt(column); // remove the columne
            }

            if (removedFrame.Content is Page { DataContext: IDisposable vmPageDataContext })
            {
                // Dispose the data model
                vmPageDataContext.Dispose();
            }

            AdjustColumnWidths();
            UpdateTopFrame();

            lock (BreadCrumbs)
            {
                if (BreadCrumbs.Count > 0)
                    BreadCrumbs.RemoveAt(BreadCrumbs.Count - 1);
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
            if (!frameStack.Any())
                return;

            var topFrame = frameStack.Peek();
            if (topFrame != null)
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
        /// <param name="stackedMode">defines how the frames are beeing added</param>
        public void Initialize(StackedMode stackedMode)
        {
            // add an empty column
            if (stackedMode == StackedMode.Resizeable)
                StackPanel.ColumnDefinitions.Add(new ColumnDefinition());

            StackedMode = stackedMode;
        }

        /// <summary>
        /// Gets or sets the stacked mode
        /// </summary>
        public StackedMode StackedMode { get; internal set; }

        #region Property SplitterWith

        public static readonly DependencyProperty SplitterWidthProperty =
            DependencyProperty.RegisterAttached("SplitterWidth", typeof(GridLength), typeof(StackedFrame), new PropertyMetadata(new GridLength(16)));

        /// <summary>
        /// Gets or sets the with of the splitter
        /// </summary>
        public GridLength SplitterWidth
        {
            get => (GridLength)GetValue(SplitterWidthProperty);
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

                // Change the visibility
                if (value == Visibility.Visible)
                {
                    SetValue(PropertyPanelVisibilityProperty, value);
                    if (FadeInDuration > 0)
                        FadeIn(PropertyPanel);
                }
                else
                {
                    if (FadeOutDuration > 0)
                    {
                        FadeOut(PropertyPanel).ContinueWith((t) =>
                        {
                            Windows.Foundation.IAsyncAction asyncAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                PropertyPanel.Opacity = 1;
                                SetValue(PropertyPanelVisibilityProperty, value);
                            });
                        });
                    }
                    else
                    {
                        SetValue(PropertyPanelVisibilityProperty, value);
                    }
                }
            }
        }
        #endregion

    }
}
