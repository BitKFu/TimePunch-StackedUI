using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;

namespace TimePunch.StackedUI.Controller
{

    public class StackedController : BaseController,
        IHandleMessageAsync<GoBackPageNavigationRequest>, IStackedController
    {
        private StackedFrame? stackedFrame;
        private StackedMode stackedMode;

        /// <summary>
        /// Creates a new instance of the StackedController
        /// </summary>
        /// <param name="eventAggregator">Event aggregation object</param>
        /// <param name="mode">Defines how the frames are added</param>
        public StackedController(IEventAggregator eventAggregator, StackedMode mode)
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
        public async Task<Page?> AddPage(Page page, Page? basePage = null, bool isResizable = true, bool isModal = true)
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

            if (StackedMode == StackedMode.FullWidth && isResizable && StackedFrame.TopFrame != null)
                StackedFrame.AddSplitter();

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

        #region Implementation of IHandleMessageAsync<GoBackPageNavigationRequest>

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
            return message;
        }

        #endregion
    }
}
