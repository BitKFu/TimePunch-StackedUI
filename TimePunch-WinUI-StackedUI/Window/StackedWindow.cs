using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;

namespace TimePunch.StackedUI.Window
{
    public class StackedWindow : Microsoft.UI.Xaml.Window
    {
        public StackedWindow()
        {
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        }

    }
}
