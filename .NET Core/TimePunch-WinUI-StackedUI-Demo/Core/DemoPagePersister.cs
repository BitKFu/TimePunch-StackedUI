using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using TimePunch.StackedUI.Model;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoPagePersister : IPagePersister
    {

        private Dictionary<string, GridLength> pageWidths = new Dictionary<string, GridLength>();

        public void SavePageWidth(string pageName, GridLength pageWidth)
        {
            pageWidths[pageName] = pageWidth;
        }

        public double GetPageWidth(string pageName)
        {
            if (pageWidths.TryGetValue(pageName, out var width))
                return width.Value;

            return double.NaN;
        }
    }
}
