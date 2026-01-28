using System;
using System.Threading;
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
        private readonly SemaphoreSlim navigationSemaphore = new(1);

        protected class DoPrevent : IDisposable
        {
            private readonly StackedController sc;

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
        /// Creates a new instance of the StackedController
        /// </summary>
        /// <param name="eventAggregator">Event aggregation object</param>
        protected StackedController(IEventAggregator eventAggregator)
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

        /// <summary>
        /// Used to navigate to a new Frame, e.g. add a frame with a new page
        /// </summary>
        protected virtual async Task<Page?> AddPage(Page page, Page? basePage = null, bool isResizable = true, bool isModal = true)
        {
            // if a base frame is set, go back to it
            if (basePage != null)
                await EventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest(basePage, false));

            // Check if the frame is already created
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame == null || StackedFrame.Contains(frameKey))
                return null;

            // if the page is modal, than disable the previous one
            if (isModal)
                StackedFrame.DisableTop();

            // It add a splitter before the new frame, if the mode is resizeable and the new frame is not the top one
            if (StackedMode == StackedMode.Resizeable && isResizable && StackedFrame.IsNextToTopFrame)
                StackedFrame.AddSplitter(StackedFrame.TopFrame);

            var isTopFrame = StackedFrame.IsTopFrame;

            // add the new page
            var frame = CreateFrame();
            await StackedFrame.AddFrame(EventAggregator, frame, page);

            if (StackedMode == StackedMode.Resizeable && isResizable && !isTopFrame)
                StackedFrame.AddSplitter(frame);

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

                if (StackedFrame.CheckAccess())
                    return StackedFrame.CanGoBack;

                var result = false;
                StackedFrame.Dispatcher.Invoke(() => result = StackedFrame.CanGoBack);
                return result;
            }
        }

        /// <summary>
        /// Goes back to the top frame
        /// </summary>
        protected async Task GoBackPageTop()
        {
            if (StackedFrame == null)
                return;

            // Save the current page width before adding a new one
            var pagePersister = GetPagePersister();
            SaveCurrentPageWidth(pagePersister);

            // Go back to top
            while (StackedFrame.TopFrame != null)
                await StackedFrame.GoBack(false);

            EventAggregator.PublishMessage(new GoBackPageTopEvent());
        }

        public virtual async Task<GoBackPageNavigationRequest> Handle(GoBackPageNavigationRequest message)
        {
            if (StackedFrame == null)
                return message;

            if (!StackedFrame.CheckAccess())
            {
                await StackedFrame.Dispatcher.Invoke(() => Handle(message));
                return message;
            }

            // wait to get the navigation handle
            if (message.TakeLock)
                await navigationSemaphore.WaitAsync();
            try
            {
                var topPage = StackedFrame.TopFrame?.Content as Page;

                // Save the current page width before adding a new one
                var pagePersister = GetPagePersister();
                SaveCurrentPageWidth(pagePersister);

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

                var newTopFrame = StackedFrame.TopFrame;
                var newTopPage = newTopFrame?.Content as Page;
                if (!StackedFrame.IsNextToTopFrame && pagePersister != null && StackedMode == StackedMode.Resizeable)
                {
                    // Get the key of the new top page - after closing the previous
                    if (newTopFrame != null && newTopPage != null)
                    {
                        var frameKey = StackedFrameExtension.GetFrameKey(newTopPage);
                        var pageWidth = pagePersister.GetPageWidth(frameKey);
                        if (pageWidth.IsAbsolute)
                            newTopFrame.Width = pageWidth.Value;
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
                    UpdateScrollPosition();

                    // Set the focus to the page
                    SetPageFocus(newTopPage);

                    // Eventually Hides the property panels
                    UpdatePropertyPanels(newTopPage);
                }
            }
            finally
            {
                if (message.TakeLock)
                    navigationSemaphore.Release();
            }

            return message;
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

        public async Task<Page?> InitTopPageAsync(PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, bool isResizable = true, bool isModal = false)
        {
            // wait to get the navigation handle
            await navigationSemaphore.WaitAsync();
            try
            {
                await GoBackPageTop();
                return await ProtectedInitSubPageAsync(message, vm, pageToAdd, null, isResizable, isModal, false);
            }
            finally
            {
                navigationSemaphore.Release();
            }
        }

        public virtual async Task<Page?> InitSubPageAsync(PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, Page? basePage = null, bool isResizable = true, bool isModal = false)
        {
            return await ProtectedInitSubPageAsync(message, vm, pageToAdd, basePage, isResizable, isModal, true);
        }

        protected virtual async Task<Page?> ProtectedInitSubPageAsync(PageNavigationEvent message, ViewModelBase vm, Page pageToAdd, Page? basePage, bool isResizable, bool isModal, bool takeLock)
        {
            if (StackedFrame == null)
                return null;

            // wait to get the navigation handle
            if (takeLock)
                await navigationSemaphore.WaitAsync();
            try
            {
                using (new DoPrevent(this))
                {
                    StackedFrame.IsLoading = true;
                    try
                    {
                        if (!await vm.InitializePageAsync(message))
                            return null;
                    }
                    finally
                    {
                        StackedFrame.IsLoading = false;
                    }

                    // Save the current page width before adding a new one
                    var pagePersister = GetPagePersister();
                    SaveCurrentPageWidth(pagePersister);

                    // Now add the page
                    var addedPage = await AddPage(pageToAdd, basePage, isResizable, isModal);
                    if (addedPage == null)
                        return null;

                    // Add scroll wheel query
                    if (addedPage is not IPreventMouseWheelScrolling) // On report page scroll wheel is used for zoom
                    {
                        if (!addedPage.IsLoaded)
                            addedPage.Loaded += (_, _) => { AddMouseWheelListener(addedPage); };
                        else
                            AddMouseWheelListener(addedPage);
                    }

                    await Task.Delay(50).ContinueWith(t => StackedFrame.Dispatcher.Invoke(() =>
                    {
                        UpdateScrollPosition();

                        // Set the focus to the page
                        SetPageFocus(addedPage);

                        // Eventually Hides the property panels
                        UpdatePropertyPanels(addedPage);
                    }));

                    return addedPage;
                }
            }
            finally
            {
                if (takeLock)
                    navigationSemaphore.Release();
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

            void OnScrollPanel(object sender, MouseWheelEventArgs e)
            {
                if (!Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down))
                    return;

                var scroll = VisualTreeHelperEx.FindDescendantByType<ScrollViewer>(StackedFrame);
                var currentOffset = scroll?.HorizontalOffset ?? 0;
                scroll?.ScrollToHorizontalOffset(currentOffset - e.Delta);
            }
        }

        protected void SaveCurrentPageWidth(IPagePersister? pagePersister)
        {
            if (StackedFrame == null)
                return;

            if (pagePersister != null && StackedMode == StackedMode.Resizeable)
            {
                // Try to store the current width
                if (StackedFrame.TopFrame?.Content is Page topPage)
                {
                    var frameKey = StackedFrameExtension.GetFrameKey(topPage);
                    pagePersister.SavePageWidth(frameKey, new GridLength(topPage.ActualWidth));
                }
            }
        }

        #endregion

        #region Scrollbar Handling

        protected virtual void UpdateScrollPosition()
        {
            StackedFrame?.UpdateScrollPosition();
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
