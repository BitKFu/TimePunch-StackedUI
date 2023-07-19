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
       , IHandleMessageAsync<NavigateToDemo1View>
       , IHandleMessageAsync<NavigateToDemo2View>
       , IHandleMessageAsync<NavigateToDemo3View>
    {
        public DemoController() 
            : base(DemoKernel.Instance.EventAggregator, StackedMode.Resizeable)
        {
        }

        #region Implementation of IHandleMessage<NavigateToDemo1View>

        public async Task<NavigateToDemo1View> Handle(NavigateToDemo1View message)
        {
            await AddPage(new Demo1View());
            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo2View>

        public async Task<NavigateToDemo2View> Handle(NavigateToDemo2View message)
        {
            await AddPage(new Demo2View());
            return message;
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo3View>

        public async Task<NavigateToDemo3View> Handle(NavigateToDemo3View message)
        {
           await AddPage(new Demo3View());
           return message;
        }

        #endregion
    }
}
