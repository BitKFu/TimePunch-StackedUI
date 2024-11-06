using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;
using TimePunch.StackedUI.Model;
using TimePunch.StackedUI.ViewModels;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

#pragma warning disable CA1416

namespace TimePunch.StackedUI.Controller
{

    public abstract class StackedController : BaseController,
        IHandleMessageAsync<GoBackPageNavigationRequest>, IStackedController
    {
        private StackedFrame? stackedFrame;
        private StackedMode stackedMode;

        /// <summary>
        /// Creates a new instance of the StackedController
        /// </summary>
        /// <param name="eventAggregator">Event aggregation object</param>
        /// <param name="mode">Defines how the frames are added</param>
        protected StackedController(IEventAggregator eventAggregator, StackedMode mode)
            : base(eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            StackedMode = mode;
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
                stackedFrame?.Initialize(StackedMode);
            }
        }

        /// <summary>
        /// Gets or sets the stacked mode
        /// </summary>
        public StackedMode StackedMode
        {
            get => stackedMode;
            set
            {
                if (value != stackedMode && StackedFrame != null)
                {
                    StackedFrame.StackedMode = value;
                }

                stackedMode = value;
            }
        }

        #region Implementation of IHandleMessage<NavigateToNewFrame>

        /// <summary>
        /// Used to navigate to a new Frame, e.g. add a frame with a new page
        /// </summary>
        private async Task<Page?> AddPage(Page page, Page? basePage, bool isResizable, bool isModal)
        {
            // if a base frame is set, go back to it
            if (basePage != null)
                await EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest(basePage));
            
            // Check if the frame is already created
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame == null || StackedFrame.Contains(frameKey))
                return null;
            
            // if the page is modal, then disable the previous one
            if (isModal)
                StackedFrame.DisableTop();
            
            //if (StackedMode == StackedMode.FullWidth && isResizable && StackedFrame.TopFrame != null)
            //    StackedFrame.AddSplitter();
            
            // add the new page
            var frame = CreateFrame();
            await StackedFrame.AddFrame(EventAggregator, frame, page);
            
            if (StackedMode == StackedMode.Resizeable && isResizable)
                StackedFrame.AddSplitter();
           
            return page;
        }

        /// <summary>
        /// Creates a new frame
        /// </summary>
        /// <returns></returns>
        protected virtual Frame CreateFrame()
        {
            return new Frame();
        }

        #endregion

        /// <summary>
        /// Abstract method to retrieve the page persister
        /// </summary>
        /// <returns></returns>
        protected abstract IPagePersister? GetPagePersister();

        #region Page Handling

        /// <summary>
        /// Gets a value indicating whether the user can go back one page
        /// </summary>
        public bool CanGoBackPage
        {
            get
            {
                if (StackedFrame == null)
                    return false;
                
                if (IsUiThread)
                    return StackedFrame.CanGoBack;

                //  Try to evaluate async
                bool result = false;

                var waitHandle = new ManualResetEvent(false);
                ApplicationDispatcher.TryEnqueue(() =>
                {
                    result = StackedFrame.CanGoBack;
                    waitHandle.Set();
                });
                waitHandle.WaitOne();

                return result;
            }
        }


        public virtual async Task<GoBackPageNavigationRequest> Handle(GoBackPageNavigationRequest message)
        {
            if (StackedFrame == null)
                return message;

            if (!IsUiThread)
            {
                var waitHandle = new ManualResetEvent(false);
                ApplicationDispatcher.TryEnqueue(async () => {
                    message = await Handle(message);
                    waitHandle.Set();
                });
                return message;
            }

            var topPage = StackedFrame.TopFrame?.Content as Page;

            // Store the grid size
            var gridWithToSave = GridLength.Auto;
            var gridMinWith = 0.0;
            if (topPage != null && StackedFrame.TopFrame?.Parent is Grid surroundingGrid)
            {
                var currentColumn = Grid.GetColumn(StackedFrame.TopFrame);
                if (currentColumn > 1 && StackedMode != StackedMode.InPlace) // if it's 0 its the last content 
                {
                    gridWithToSave = surroundingGrid.ColumnDefinitions[currentColumn - 2].Width;
                    gridMinWith = surroundingGrid.ColumnDefinitions[currentColumn - 2].MinWidth;
                }
                else if (currentColumn > 0 && StackedMode == StackedMode.InPlace)
                {
                    // Inplace don't store
                }
                else
                {
                    // Prevent going back to a blank page
                    if (message.ToPage == null)
                        return message;
                }
            }

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

            // Get the key of the new top page - after closing the previous
            var newTopPage = StackedFrame?.TopFrame?.Content as Page;
            if (newTopPage != null && newTopPage != topPage && StackedMode != StackedMode.InPlace)
            {
                var pagePersister = GetPagePersister();
                if (pagePersister != null)
                {
                    var frameKey = StackedFrameExtension.GetFrameKey(newTopPage);
                    if (Math.Abs(gridWithToSave.Value - gridMinWith) > 1)
                        pagePersister.SavePageWidth(frameKey, gridWithToSave);
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

        public async Task<Page?> InitTopPageAsync(DispatcherQueue dispatcher, PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, bool isResizable = true, bool isModal = false)
        {
            await GoBackPageTop();
            return await InitSubPageAsync(dispatcher, message, vm, pageToAdd, null, isResizable, isModal);
        }

        public virtual async Task<Page?> InitSubPageAsync(DispatcherQueue dispatcher, PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, Page? basePage = null, bool isResizable = true, bool isModal = false)
        {
            if (StackedFrame == null)
                return null;

            using (new DoPrevent(this))
            {
                if (!await vm.InitializePageAsync(message, dispatcher))
                    return null;
                
                // Try to read the saved width
                var pageWidth = pageToAdd.Width;
                if (StackedFrame.TopFrame?.Content is Page topPage)
                {
                    var pagePersister = GetPagePersister();
                    if (pagePersister != null)
                    {
                        var frameKey = StackedFrameExtension.GetFrameKey(topPage);
                        pageWidth = pagePersister.GetPageWidth(frameKey);
                        pageToAdd.Width = pageWidth;
                    }
                }

                // Now add the page
                var addedPage = await AddPage(pageToAdd, basePage, isResizable, isModal);
                if (addedPage == null)
                    return null;

                // wait a short moment to be sure, that the page has been displayed
                await Task.Delay(50).ContinueWith(t => dispatcher.TryEnqueue(()=>
                    {
                        UpdateScrollPosition(addedPage);

                        // Set the focus to the page
                        SetPageFocus(addedPage);

                        // Eventually Hides the property panels
                        UpdatePropertyPanels(addedPage);
                    }));

                return addedPage;
            }

        }

        #endregion

        #region Scrollbar Handling

        protected virtual void UpdateScrollPosition(Page addedPage)
        {
            if (StackedFrame == null)
                return;

            // we only need to scroll to the end, if the page does not contain a grid!
            var scroll = StackedFrame.FindDescendant<ScrollViewer>(); 
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

        #region Property Panels

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

        #endregion
    }
}
