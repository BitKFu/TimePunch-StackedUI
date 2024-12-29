using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;
using TimePunch.StackedUI.Model;
using TimePunch.StackedUI.ViewModels;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace TimePunch.StackedUI.Controller
{

    public abstract class StackedController : BaseController,
        IHandleMessageAsync<GoBackPageNavigationRequest>, IStackedController
    {
        private StackedFrame? stackedFrame;

        /// <summary>
        /// Creates a new instance of the StackedController
        /// </summary>
        /// <param name="eventAggregator">Event aggregation object</param>
        /// <param name="mode">Defines how the frames are added</param>
        public StackedController(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
        }

        /// <summary>
        /// Stacked Frame control
        /// </summary>
        public StackedFrame? StackedFrame
        {
            get => stackedFrame;
            set
            {
                stackedFrame = value;
                stackedFrame?.Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the stacked mode
        /// </summary>
        public StackedMode StackedMode
        {
            get => StackedFrame?.StackedMode ?? StackedMode.Resizeable;
            set
            {
                if (StackedFrame != null)
                    StackedFrame.StackedMode = value;
            }
        }

        #region Implementation of IHandleMessage<NavigateToNewFrame>

        /// <summary>
        /// Used to navigate to a new Frame, e.g. add a frame with a new page
        /// </summary>
        protected virtual async Task<Page?> AddPage(Page page, Page? basePage = null, bool isResizable = true, bool isModal = true)
        {
            // if a base frame is set, go back to it
            if (basePage != null)
                await EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest(basePage));

            // Check if the frame is already created
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame == null || StackedFrame.Contains(frameKey))
                return null;

            // if the page is modal, than disable the previous one
            if (isModal)
                StackedFrame.DisableTop();

            // add the new page
            var frame = CreateFrame();
            await StackedFrame.AddFrame(EventAggregator, frame, page);

            if (StackedMode == StackedMode.Resizeable && isResizable)
                StackedFrame.AddSplitter();

            return page;
        }

        protected class DoPrevent : IDisposable
        {
            private StackedController sc;

            public DoPrevent(StackedController sc)
            {
                this.sc = sc;
                sc.PreventReset = true;
            }

            public void Dispose()
            {
                sc.PreventReset = false;
            }
        }


        private int preventResetCounter = 0;

        /// <summary>
        /// Smart Counter for preventing search
        /// </summary>
        protected bool PreventReset
        {
            get => preventResetCounter > 0;
            set
            {
                if (value)
                    preventResetCounter++;
                else
                    preventResetCounter--;
            }
        }

        /// <summary>
        /// Goes back to the top frame
        /// </summary>
        protected async Task GoBackPageTop()
        {
            if (StackedFrame == null)
                return;

            // Go back to top
            while (StackedFrame.TopFrame != null)
                await StackedFrame.GoBack(false);

            EventAggregator.PublishMessage(new GoBackPageTopEvent());
        }

        /// <summary>
        /// Goes back to the top content frame
        /// </summary>
        protected void GoBackContentTop()
        {
            if (ContentFrame == null)
                return;

            // Go back to top
            while (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }

        public async Task<Page?> InitTopPageAsync( PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, bool isResizable = true, bool isModal = false)
        {
            await GoBackPageTop();
            return await InitSubPageAsync(message, vm, pageToAdd, null, isResizable, isModal);
        }

        public virtual async Task<Page?> InitSubPageAsync(PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, Page? basePage = null, bool isResizable = true, bool isModal = false)
        {
            if (StackedFrame == null)
                return null;

            using (new DoPrevent(this))
            {
                if (!await vm.InitializePageAsync(message))
                    return null;

                // Save the current page width before adding a new one
                var pagePersister = GetPagePersister();
                if (pagePersister != null && StackedMode == StackedMode.Resizeable)
                {
                    // Try to store the current width
                    string frameKey;
                    if (StackedFrame.TopFrame?.Content is Page topPage)
                    {
                        frameKey = StackedFrameExtension.GetFrameKey(topPage);
                        pagePersister.SavePageWidth(frameKey, new GridLength(topPage.ActualWidth));
                    }

                    // Try to read the saved width
                    frameKey = StackedFrameExtension.GetFrameKey(pageToAdd);
                    var pageWidth = pagePersister.GetPageWidth(frameKey);
                    if (pageWidth.IsAbsolute)
                        pageToAdd.Width = pageWidth.Value;
                }

                // Now add the page
                var addedPage = await AddPage(pageToAdd, basePage, isResizable, isModal);
                if (addedPage == null)
                    return null;

                // Add scroll wheel query
                var useScrollWheel = addedPage is not IPreventMouseWheelScrolling;       // On report page scroll wheel is used for zoom

                if (useScrollWheel)
                {
                    if (!addedPage.IsLoaded)
                        addedPage.Loaded += (_, _) => { AddMouseWheelListener(addedPage); };
                    else
                        AddMouseWheelListener(addedPage);
                }

                UpdateScrollPosition(addedPage);

                // Set the focus to the page
                SetPageFocus(addedPage);

                // Eventually Hides the property panels
                UpdatePropertyPanels(addedPage);

                return addedPage;
            }

            void AddMouseWheelListener(UIElement element)
            {
                element.MouseWheel += OnScrollPanel;

                var childElements = VisualTreeHelper.GetChildrenCount(element);
                for (var x = 0; x < childElements; x++)
                {
                    var child = VisualTreeHelper.GetChild(element, x);
                    if (child is UIElement uiElement)
                        AddMouseWheelListener(uiElement);
                }
            }
        }

        private void OnScrollPanel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down))
                return;

            var scroll = VisualTreeHelperEx.FindDescendantByType<ScrollViewer>(StackedFrame);
            var currentOffset = scroll?.HorizontalOffset ?? 0;
            scroll?.ScrollToHorizontalOffset(currentOffset - e.Delta);
        }

        /// <summary>
        /// Creates a new frame
        /// </summary>
        /// <returns></returns>
        protected virtual Frame CreateFrame()
        {
            return new Frame();
        }

        /// <summary>
        /// Abstract method to retrieve the page persister
        /// </summary>
        /// <returns></returns>
        protected abstract IPagePersister? GetPagePersister();


        #endregion


        #region Scrollbar Handling

        protected virtual void UpdateScrollPosition(Page addedPage)
        {
            if (StackedFrame == null)
                return;

            // we only need to scroll to the end, if the page does not contain a grid!
            var scroll = VisualTreeHelperEx.FindDescendantByType<ScrollViewer>(StackedFrame);
            if (scroll == null)
                return;

            // Get the key of the new top page - to set the with
            if (StackedFrame.TopFrame?.Parent is StackPanel panel)
            {
                var currentColumn = panel.Children.IndexOf(StackedFrame.TopFrame);
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

        #endregion

        #region Focus Helper

        /// <summary>
        /// Used to set the focus in the page
        /// </summary>
        /// <param name="addedPage"></param>
        protected abstract void SetPageFocus(Page addedPage);

        #endregion

        /// <summary>
        /// Set the content of the property panel
        /// </summary>
        /// <param name="content">Content</param>
        public virtual void ShowPropertyPanel(UIElement content)
        {
            if (StackedFrame == null)
                return;

            StackedFrame.PropertyPanel.Children.Clear();
            StackedFrame.PropertyPanel.Children.Add(content);

            StackedFrame.PropertyPanelVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the property panel
        /// </summary>
        public virtual void HidePropertyPanel()
        {
            if (StackedFrame == null)
                return;
            StackedFrame.PropertyPanelVisibility = Visibility.Collapsed;
            StackedFrame.PropertyPanel.Children.Clear();
        }

        public UIElement? PropertyPanel
        {
            get => StackedFrame?.PropertyPanel.Children.Count > 0
                ? StackedFrame.PropertyPanel.Children[0]
                : null;

            set
            {
                if (value != null)
                    ShowPropertyPanel(value);
                else
                    HidePropertyPanel();
            }
        }


        /// <summary>
        /// Used to hide the property panels
        /// </summary>
        /// <param name="newTopPage"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected abstract void UpdatePropertyPanels(Page newTopPage);

        /// <summary>
        /// Gets a value indicating whether the user can go back one page
        /// </summary>
        public bool CanGoBackPage
        {
            get
            {
                if (StackedFrame == null)
                    return false;

                if (StackedFrame.CheckAccess())
                    return StackedFrame.CanGoBack;

                var result = false;
                StackedFrame.Dispatcher.Invoke(() => result = StackedFrame.CanGoBack);
                return result;
            }
        }

        #region Implementation of IHandleMessageAsync<GoBackPageNavigationRequest>

        public virtual async Task<GoBackPageNavigationRequest> Handle(GoBackPageNavigationRequest message)
        {
            if (StackedFrame == null)
                return message;

            if (!StackedFrame.CheckAccess())
            {
                await StackedFrame.Dispatcher.Invoke(() => Handle(message));
                return message;
            }

            var topPage = StackedFrame.TopFrame?.Content as Page;

            //// Store the grid size
            //var gridWithToSave = GridLength.Auto;
            //var gridMinWith = 0.0;
            //if (topPage != null && StackedFrame.TopFrame?.Parent is Grid surroundingGrid)
            //{
            //    var currentColumn = Grid.GetColumn(StackedFrame.TopFrame);
            //    if (currentColumn > 1 && StackedMode != StackedMode.InPlace) // if it's 0 its the last content 
            //    {
            //        gridWithToSave = surroundingGrid.ColumnDefinitions[currentColumn - 2].Width;
            //        gridMinWith = surroundingGrid.ColumnDefinitions[currentColumn - 2].MinWidth;
            //    }
            //    else if (currentColumn > 0 && StackedMode == StackedMode.InPlace)
            //    {
            //        // Inplace don't store
            //    }
            //    else
            //    {
            //        // Prevent going back to a blank page
            //        if (message.ToPage == null)
            //            return message;
            //    }

            //}

            // Remove the top frame
            if (message.ToPage == null)
                await StackedFrame.GoBack(true);
            else
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                while (StackedFrame.TopFrame != null && StackedFrame.TopFrame.Content != message.ToPage)
                    await StackedFrame.GoBack(true);
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            }

            StackedFrame.EnableTop();

            var newTopFrame = StackedFrame?.TopFrame;
            var newTopPage = newTopFrame?.Content as Page;
            var pagePersister = GetPagePersister();
            if (pagePersister != null && StackedMode == StackedMode.Resizeable)
            {
                // Get the key of the new top page - after closing the previous
                if (newTopFrame != null && newTopPage != null)
                {
                    var frameKey = StackedFrameExtension.GetFrameKey(newTopPage);
                    var newWidth = pagePersister.GetPageWidth(frameKey);
                    var currentColumn = Grid.GetColumn(newTopFrame);
                    if (StackedMode != StackedMode.InPlace && newTopFrame?.Parent is Grid surroundingGrid) // if it's 0 its the last content
                    {
                        surroundingGrid.ColumnDefinitions[currentColumn].Width = newWidth;
                    }
                }
            }

            // Reset the last fired command, if the user goes back with breadcrumb
            if (message.ToPage != topPage && message.ToPage?.DataContext is StackedViewModelBase vm)
            {
                if (!PreventReset)      // This will be true, if the a sub dialog gets opened due to a internal page change
                    vm.ResetLastFiredCommand();
            }
            else
            {
                // Reset the last fired command, if the dialog gets closed
                if (message.ToPage == null)
                {
                    topPage = StackedFrame?.TopFrame?.Content as Page;
                    if (topPage?.DataContext is StackedViewModelBase vm2)
                        vm2.ResetLastFiredCommand();
                }
            }

            // Maybe closed a page and the underlying page contains a grid, then the grid needs the focus
            if (newTopPage != null)
            {
                UpdateScrollPosition(newTopPage);

                // Set the focus to the page
                SetPageFocus(newTopPage);

                // Eventually Hides the property panels
                UpdatePropertyPanels(newTopPage);
            }

            return message;
        }

        #endregion
    }
}
