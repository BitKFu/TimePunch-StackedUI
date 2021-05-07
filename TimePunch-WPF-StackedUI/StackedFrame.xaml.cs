using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TimePunch.MVVM.EventAggregation;
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

        /// <summary>
        /// This method sets only the last column definition to star width
        /// </summary>
        private void AdjustColumnWidths()
        {
            for (int i = 0; i < StackPanel.ColumnDefinitions.Count; i++)
            {
                var last = i == StackPanel.ColumnDefinitions.Count - 1;

                if (!last && StackPanel.ColumnDefinitions[i].Width.IsStar)
                    StackPanel.ColumnDefinitions[i].Width = GridLength.Auto;
                else
                {
                    if (last && StackPanel.ColumnDefinitions[i].Width.IsAuto)
                        StackPanel.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                }
            }
            ScrollViewer.ScrollToRightEnd();
        }

        public void AddFrame(IEventAggregator eventAggregator, Frame frame, Page page)
        {
            // add a new column
            var column = StackPanel.ColumnDefinitions.Count;
            StackPanel.ColumnDefinitions.Insert(column, new ColumnDefinition() { Width = GridLength.Auto, MinWidth = page.MinWidth });
            Grid.SetColumn(frame, column);

            // push the frame to the new column
            frame.Content = page;
            frameStack.Push(frame);
            StackPanel.Children.Add(frame);

            AdjustColumnWidths();
            UpdateTopFrame();

            BreadCrumbs.Add(new BreadCrumbNavigation(eventAggregator, page));
        }

        public void AddSplitter()
        {
            var splitter = new GridSplitter
            {
                ShowsPreview = false,
                ResizeDirection = GridResizeDirection.Columns,
                ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // add the splitter 
            var column = StackPanel.ColumnDefinitions.Count;
            StackPanel.ColumnDefinitions.Insert(column, new ColumnDefinition() { Width = new GridLength(5) });
            Grid.SetColumn(splitter, column);

            StackPanel.Children.Add(splitter);

            // push the splitter to the frame dictionary
            splitters.Add(frameStack.Peek(), splitter);
        }

        public void GoBack()
        {
            if (frameStack.Count == 0)
                return;

            // remove the frame
            var removedFrame = frameStack.Pop();            // get the frame to remove
            var column = Grid.GetColumn(removedFrame);      // get the column in grid to remove
            StackPanel.Children.Remove(removedFrame);       // remove the frame
            StackPanel.ColumnDefinitions.RemoveAt(column);  // remove the columne

            // remove the splitter if there is one
            var removedSplitter = frameStack.Any() ? frameStack.Peek() : null;
            if (removedSplitter != null && splitters.ContainsKey(removedSplitter))
            {
                var splitter = splitters[removedSplitter];         // get the splitter to remove
                column = Grid.GetColumn(splitter);              // get the column in grid to remove
                StackPanel.Children.Remove(splitter);           // remove the splitter
                splitters.Remove(removedSplitter);                 // remove the splitter / frame binding
                StackPanel.ColumnDefinitions.RemoveAt(column);  // remove the columne
            }

            AdjustColumnWidths();
            UpdateTopFrame();
            BreadCrumbs.RemoveAt(BreadCrumbs.Count-1);
        }

        private void UpdateTopFrame()
        {
            CanGoBack = frameStack.Count()>1;
            TopFrame = frameStack.Any() ? frameStack.Peek() : null;
        }

        public void DisableTop()
        {
            var topFrame = frameStack.Count > 0 ? frameStack.Peek() : null;
            if (topFrame != null)
                topFrame.IsHitTestVisible = false;
        }
        public void EnableTop()
        {
            if (!frameStack.Any())
                return;

            var topFrame = frameStack.Peek();
            if (topFrame != null)
                topFrame.IsHitTestVisible = true;
        }

        public bool Contains(string frameKey)
        {
            foreach (var frame in frameStack)
            {
                var key = StackedFrameExtension.GetFrameKey(frame.Content as Page);
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

    }
}
