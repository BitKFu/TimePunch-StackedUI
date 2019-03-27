using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;

namespace TimePunch.StackedUI.Demo.Core
{
    public class DemoViewModelBase : ViewModelBase
    {
        public DemoViewModelBase() : base(DemoKernel.Instance.EventAggregator)
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
