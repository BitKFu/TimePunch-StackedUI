﻿using System;
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
        public Page AddPage(Page page, Page? basePage = null, bool isResizable = true, bool isModal = true)
        {
            // if a base frame is set, go back to it
            if (basePage != null)
                EventAggregator.PublishMessage(new GoBackPageNavigationRequest(basePage));

            // Check if the frame is already created
            var frameKey = StackedFrameExtension.GetFrameKey(page);
            if (StackedFrame.Contains(frameKey))
                return null;

            // if the page is modal, than disable the previous one
            if (isModal)
                StackedFrame.DisableTop();

            if (isResizable && StackedFrame.TopFrame != null)
                StackedFrame.AddSplitter();

            // add the new page
            var frame = CreateFrame();
            StackedFrame.AddFrame(EventAggregator, frame, page);
            
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

        #region Implementation of IHandleMessage<GoBackPageNavigationRequest>

        public virtual void Handle(GoBackPageNavigationRequest message)
        {
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
    }
}
