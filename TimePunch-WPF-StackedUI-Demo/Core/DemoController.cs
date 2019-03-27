using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Demo.Views;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoController : StackedController
       ,IHandleMessage<NavigateToDemo1View>
       ,IHandleMessage<NavigateToDemo2View>
       ,IHandleMessage<NavigateToDemo3View>
       ,IHandleMessage<NavigateToStartView>
    {
        public DemoController() 
            : base(DemoKernel.Instance.EventAggregator)
        {
        }

        #region Implementation of IHandleMessage<NavigateToDemo1View>

        public void Handle(NavigateToDemo1View message)
        {
            AddPage(new Demo1View());
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToStartView>

        public void Handle(NavigateToStartView message)
        {
            AddPage(new StartView());
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo2View>

        public void Handle(NavigateToDemo2View message)
        {
            AddPage(new Demo2View());
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo3View>

        public void Handle(NavigateToDemo3View message)
        {
            AddPage(new Demo3View());
        }

        #endregion
    }
}
