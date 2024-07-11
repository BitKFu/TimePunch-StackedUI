using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
using TimePunch.MVVM.Commands;
using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controls;
using TimePunch.StackedUI.Events;
using TimePunch.StackedUI.Extensions;

namespace TimePunch.StackedUI.Model
{
    public class BreadCrumbNavigation
    {
        public BreadCrumbNavigation(IEventAggregator eventAggregator, Page page)
        {
            FrameKey = StackedFrameExtension.GetFrameKey(page);

            var pageControl = page.Content as PageControl;
            if (pageControl != null)
                FrameTitle = pageControl.GetValue(PageControl.TitleProperty).ToString();

            Command = new DynamicCommand(
                (sender) => eventAggregator.PublishMessageAsync(new GoBackPageNavigationRequest(page)));
        }

        public string FrameKey { get; }
        public string FrameTitle { get; } = string.Empty;
        
        public ICommand Command { get; }
    }
}
