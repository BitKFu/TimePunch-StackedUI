using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.StackedUI.ViewModels;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoViewModelBase : StackedViewModelBase
    {
        public DemoViewModelBase() 
            : base(DemoKernel.Instance.EventAggregator)
        {
        }

        #region Overrides of ViewModelBase

        public override void Initialize()
        {
        }

        public override void InitializePage(object extraData)
        {
        }

        #endregion
    }
}
