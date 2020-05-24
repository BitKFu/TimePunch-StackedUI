using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            // add an empty column
            StackPanel.ColumnDefinitions.Add(new ColumnDefinition());
        }

        public void AddFrame(Frame frame, double minWidth)
        {
            // add a new column
            var column = StackPanel.ColumnDefinitions.Count - 1;
            StackPanel.ColumnDefinitions.Insert(column, new ColumnDefinition(){Width = GridLength.Auto, MinWidth = minWidth});
            Grid.SetColumn(frame, column);

            // push the frame to the new column
            frameStack.Push(frame);
            StackPanel.Children.Add(frame);
        }

        public void AddSplitter()
        {
            var splitter = new GridSplitter
            {
                ShowsPreview = false,
                ResizeDirection = GridResizeDirection.Columns,
                ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // add the splitter
            var column = StackPanel.ColumnDefinitions.Count - 1;
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
            if (splitters.ContainsKey(removedFrame))
            {
                var splitter = splitters[removedFrame];         // get the splitter to remove
                column = Grid.GetColumn(splitter);              // get the column in grid to remove
                StackPanel.Children.Remove(splitter);           // remove the splitter
                splitters.Remove(removedFrame);                 // remove the splitter / frame binding
                StackPanel.ColumnDefinitions.RemoveAt(column);  // remove the columne
            }
        }

        public void DisableTop()
        {
            var topFrame = frameStack.Count>0 ? frameStack.Peek() : null;
            if (topFrame != null)
                topFrame.IsHitTestVisible = false;
        }
        public void EnableTop()
        {
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

        public BreadCrumbNavigation[] BreadCrumbNavigation => frameStack
            .Select(s => new BreadCrumbNavigation(
                StackedFrameExtension.GetFrameKey(s.Content as Page), 
                (s.Content as Page).GetValue(Page.TitleProperty).ToString() ))
            .Reverse()
            .ToArray();
    }
}
