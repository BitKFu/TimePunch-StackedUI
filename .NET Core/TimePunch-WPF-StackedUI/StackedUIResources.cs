using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimePunch.StackedUI
{
    public class StackedUIResources : ResourceDictionary
    {
        public StackedUIResources()
        {
            Source = new Uri("pack://application:,,,/TpStackedUI;component/StackedUIResources.xaml");
        }
    }
}
