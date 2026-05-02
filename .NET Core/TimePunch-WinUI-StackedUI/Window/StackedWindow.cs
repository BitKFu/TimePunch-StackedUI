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
#if !(__WASM__)
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
#endif
        }

    }
}
