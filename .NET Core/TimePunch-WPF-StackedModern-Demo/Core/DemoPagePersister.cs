using System.Collections.Generic;
using System.Windows;
using TimePunch.StackedUI.Model;

namespace TimePunch_WPF_StackedModern_Demo.Core
{
    public class DemoPagePersister : IPagePersister
    {

        private Dictionary<string, GridLength> pageWidths = new Dictionary<string, GridLength>();

        public void SavePageWidth(string pageName, GridLength pageWidth)
        {
            pageWidths[pageName] = pageWidth;
        }

        public GridLength GetPageWidth(string pageName)
        {
            if (pageWidths.TryGetValue(pageName, out var width))
                return width;

            return GridLength.Auto;
        }
    }
}
