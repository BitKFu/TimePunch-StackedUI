using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;

namespace TimePunch.StackedUI.Controller
{

    public class StackedController : BaseController,
        IHandleMessage<GoBackPageNavigationRequest>, IStackedController
    {
        private StackedFrame stackedFrame;

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
        public StackedFrame StackedFrame
        {
            get
            {
                return stackedFrame;
            }
            set
            {
                stackedFrame = value;
                stackedFrame.Initialize(StackedMode);
            }
        }

        /// <summary>
        /// Gets or sets the stacked mode
        /// </summary>
        public StackedMode StackedMode { get; }

        #region Implementation of IHandleMessage<NavigateToNewFrame>

        /// <summary>
        /// Used to navigate to a new Frame, e.g. add a frame with a new page
        /// </summary>
        public Page? AddPage(Page page, Page? basePage = null, bool isResizable = true, bool isModal = true)
        {
            // if a base frame is set, go back to it
            if (basePage != null)
                EventAggregator.PublishMessage(new GoBackPageNavigationRequest(basePage));

            // Check if the frame is already created
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame.Contains(frameKey))
                return null;

            // Hide the current property panel
            HidePropertyPanel();

            // if the page is modal, than disable the previous one
            if (isModal)
                StackedFrame.DisableTop();

            if (StackedMode == StackedMode.FullWidth && isResizable && StackedFrame.TopFrame != null)
                StackedFrame.AddSplitter();

            // add the new page
            var frame = CreateFrame();
            StackedFrame.AddFrame(EventAggregator, frame, page);

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
        /// <param name="width">Width</param>
        public void ShowPropertyPanel(UIElement content, double width)
        {
            StackedFrame.PropertyPanel.Children.Clear();
            StackedFrame.PropertyPanel.Children.Add(content);

            StackedFrame.PropertyPanelVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the property panel
        /// </summary>
        public void HidePropertyPanel()
        {
            StackedFrame.PropertyPanel.Children.Clear();
            StackedFrame.PropertyPanelVisibility = Visibility.Collapsed;
        }

        #region Implementation of IHandleMessage<GoBackPageNavigationRequest>

        public virtual void Handle(GoBackPageNavigationRequest message)
        {
            if (!StackedFrame.CheckAccess())
            {
                StackedFrame.Dispatcher.Invoke(() => Handle(message));
                return;
            }

            // Remove the top frame
            if (message.ToPage == null)
                StackedFrame.GoBack();
            else
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                while (StackedFrame.TopFrame != null && StackedFrame.TopFrame.Content != message.ToPage)
                    StackedFrame.GoBack();
            }

            StackedFrame.EnableTop();
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether the user can go back one page
        /// </summary>
        public bool CanGoBackPage
        {
            get
            {
                if (StackedFrame.CheckAccess())
                    return StackedFrame.CanGoBack;

                bool result = false;
                StackedFrame.Dispatcher.Invoke(() => result = StackedFrame.CanGoBack);
                return result;
            }
        }
    }
}
