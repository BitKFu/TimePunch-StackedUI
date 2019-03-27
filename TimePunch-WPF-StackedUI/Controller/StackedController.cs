using System;
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
        }

        /// <summary>
        /// Stacked Frame control
        /// </summary>
        public StackedFrame StackedFrame { get; set; }

        #region Implementation of IHandleMessage<NavigateToNewFrame>

        /// <summary>
        /// Used to navigate to a new Frame, e.g. add a frame with a new page
        /// </summary>
        public void AddPage(Page page, bool isResizable = true, bool isModal = true)
        {
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame.Contains(frameKey))
                return;

            // if the page is modal, than disable the previous one
            if (isModal)
                StackedFrame.DisableTop();

            // update the breadcrumb navigation
            page.Loaded += (s, e) => EventAggregator.PublishMessage(new UpdateBreadCrumbNavigation(StackedFrame.BreadCrumbNavigation));

            // add the new page
            var frame = new Frame {Content = page};
            StackedFrame.AddFrame(frame, page.MinWidth);

            if (isResizable)
                StackedFrame.AddSplitter();
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
