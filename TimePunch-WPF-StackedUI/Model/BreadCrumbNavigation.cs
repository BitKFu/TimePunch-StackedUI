using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TimePunch.MVVM.Commands;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;

namespace TimePunch.StackedUI.Model
{
    public class BreadCrumbNavigation
    {
        public BreadCrumbNavigation(IEventAggregator eventAggregator, Page page)
        {
            FrameKey = StackedFrameExtension.GetFrameKey(page);
            FrameTitle = page.GetValue(Page.TitleProperty)?.ToString() ?? string.Empty;
            
            Command = new DynamicCommand(
                (sender) => eventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest(page)));
        }

        public string FrameKey { get; }
        public string FrameTitle { get; }
        
        public ICommand Command { get; }
    }
}
