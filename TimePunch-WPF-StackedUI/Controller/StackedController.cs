using System;
using System.Diagnostics;
using System.Windows.Controls;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;

namespace TimePunch.StackedUI.Controller
{
    public class StackedController : BaseController,
        IHandleMessage<GoBackPageNavigationRequest>
    {
        public StackedController(IEventAggregator eventAggregator) 
            : base(eventAggregator)
        {
            if (eventAggregator == null) 
                throw new ArgumentNullException(nameof(eventAggregator));
        }

        /// <summary>
        /// Stacked Frame control
        /// </summary>
        public StackedFrame StackedFrame { get; set; }

        #region Implementation of IHandleMessage<NavigateToNewFrame>

        /// <summary>
        /// Used to navigate to a new Frame, e.g. add a frame with a new page
        /// </summary>
        public Frame? AddPage(Page page, bool isResizable = true, bool isModal = true, Frame? baseFrame = null)
        {
            // if a base frame is set, go back to it
            if (baseFrame != null)
            {
                while(StackedFrame.TopFrame != baseFrame)
                    EventAggregator.PublishMessage(new GoBackPageNavigationRequest());
            }

            // Check if the frame is already created
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame.Contains(frameKey))
                return null;

            // if the page is modal, than disable the previous one
            if (isModal)
                StackedFrame.DisableTop();

            // update the breadcrumb navigation
            page.Loaded += (s, e) => EventAggregator.PublishMessage(new UpdateBreadCrumbNavigation(StackedFrame.BreadCrumbNavigation));

            if (isResizable && StackedFrame.TopFrame != null)
                StackedFrame.AddSplitter();

            // add the new page
            var frame = CreateFrame();
            frame.Content= page;
            StackedFrame.AddFrame(frame, page.MinWidth);

            return frame;
        }

        public void GoBackTo(Frame? baseFrame)
        {
            while (StackedFrame.TopFrame != baseFrame)
                EventAggregator.PublishMessage(new GoBackPageNavigationRequest());
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

        #region Implementation of IHandleMessage<GoBackPageNavigationRequest>

        public virtual void Handle(GoBackPageNavigationRequest message)
        {
            // Remove the top frame
            StackedFrame.GoBack();
            StackedFrame.EnableTop();

            // update the breadcrumb navigation
            EventAggregator.PublishMessage(new UpdateBreadCrumbNavigation(StackedFrame.BreadCrumbNavigation));
        }

        #endregion
    }
}
