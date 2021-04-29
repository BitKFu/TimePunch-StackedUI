using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Events;
using TimePunch.StackedUI.Demo.Views;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoController : StackedController
       , IHandleMessage<NavigateToDemo1View>
       ,IHandleMessage<NavigateToDemo2View>
       ,IHandleMessage<NavigateToDemo3View>
       ,IHandleMessage<NavigateToStartView>
    {
        private Frame baseFrame;

        public DemoController() 
            : base(DemoKernel.Instance.EventAggregator)
        {
        }

        #region Implementation of IHandleMessage<NavigateToDemo1View>

        public void Handle(NavigateToDemo1View message)
        {
            GoBackTo(baseFrame);
            AddPage(new Demo1View(), true, false, baseFrame);
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToStartView>

        public void Handle(NavigateToStartView message)
        {
            var startView = new StartView();
            (startView.DataContext as ViewModelBase).InitializePage(null);

            baseFrame = AddPage(startView);
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo2View>

        public void Handle(NavigateToDemo2View message)
        {
            AddPage(new Demo2View(), true, false, baseFrame);
        }

        #endregion

        #region Implementation of IHandleMessage<NavigateToDemo3View>

        public void Handle(NavigateToDemo3View message)
        {
            AddPage(new Demo3View());
        }

        #endregion

        #region Overrides of StackedController

        protected override Frame CreateFrame()
        {
            return new ModernWpf.Controls.Frame();
        }

        #endregion
    }
}
