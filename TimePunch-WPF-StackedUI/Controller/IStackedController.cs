using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimePunch.MVVM.Controller;

namespace TimePunch.StackedUI.Controller
{
    public interface IStackedController : IBaseController
    {
        // Methods
        Page AddPage(Page page, Page basePage = null, bool isResizable = true, bool isModal = true);
        void HidePropertyPanel();
        void ShowPropertyPanel(UIElement content, double width);

        // Properties
        StackedFrame StackedFrame { get; set; }
        StackedMode StackedMode { get; }
        bool CanGoBackPage { get; }
    }

}
