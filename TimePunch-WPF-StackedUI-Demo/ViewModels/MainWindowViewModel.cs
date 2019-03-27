using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Demo.Core;
using TimePunch.StackedUI.Events;

namespace TimePunch.StackedUI.Demo.ViewModels
{
    public class MainWindowViewModel : DemoViewModelBase,
        IHandleMessage<UpdateBreadCrumbNavigation>
    {
        #region Property BreadCrumbNavigationTitle

        /// <summary>
        /// Gets or sets the BreadCrumbNavigationTitle.
        /// </summary>
        /// <value>The BreadCrumbNavigationTitle.</value>
        public string BreadCrumbNavigationTitle
        {
            get { return GetPropertyValue(() => BreadCrumbNavigationTitle); }
            set { SetPropertyValue(() => BreadCrumbNavigationTitle, value); }
        }

        #endregion

        #region Implementation of IHandleMessage<UpdateBreadCrumbNavigation>

        public void Handle(UpdateBreadCrumbNavigation message)
        {
            BreadCrumbNavigationTitle = string.Join(" -> ", message.BreadCrumbs.Select(b => b.FrameTitle));
        }

        #endregion
    }
}
