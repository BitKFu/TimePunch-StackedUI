﻿using TimePunch.MVVM.Controller;
using TimePunch.StackedUI.ViewModels;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoViewModelBase : StackedViewModelBase
    {
        public DemoViewModelBase() : base(Kernel.Instance.EventAggregator)
        {
        }

        public override void Initialize()
        {
        }
    }
}
