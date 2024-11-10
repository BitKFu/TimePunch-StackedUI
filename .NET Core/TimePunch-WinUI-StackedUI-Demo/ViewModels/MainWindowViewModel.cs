using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Dispatching;
using TimePunch.MVVM.Events;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch_WinUI_StackedUI_Demo.Events;

namespace TimePunch_WinUI_StackedUI_Demo.ViewModels
{
    public class PageLink
    {
        public string Title { get; set; } = "undefined";
        public string? Icon { get; set; }

        public Action? GoToPage { get; set; }
    }


    public class MainWindowViewModel : DemoViewModelBase
    {
        #region Overrides of DemoViewModelBase

        public override void Initialize()
        {
            base.Initialize();

            Demo1Command = RegisterCommand(ExecuteDemo1Command, CanExecuteDemo1Command, true);

            DemoPages = new List<PageLink>()
            {
                new() {Title = "Demo1", Icon = "Play", GoToPage = ()=>EventAggregator.PublishMessageAsync(new NavigateToDemo1View())},
                new() {Title = "Demo2", Icon = "Save", GoToPage = ()=>EventAggregator.PublishMessageAsync(new NavigateToDemo2View())},
            };

        }

        public override Task<bool> InitializePageAsync(object extraData, DispatcherQueue dispatcher)
        {
            var result = base.InitializePageAsync(extraData, dispatcher);
            EventAggregator.PublishMessageAsync(new NavigateToStartView());
            return result;
        }

        #endregion

        #region Property SelectedMenuItem

        /// <summary>
        /// Gets or sets the SelectedMenuItem.
        /// </summary>
        /// <value>The SelectedMenuItem.</value>
        public PageLink? SelectedMenuItem
        {
            get { return GetPropertyValue(() => SelectedMenuItem); }
            set
            {
                if (SetPropertyValue(() => SelectedMenuItem, value))
                {
                    SelectedMenuItem?.GoToPage?.Invoke();
                }
            }
        }

        #endregion

        #region Demo1 Command

        /// <summary>
        /// Gets or sets the Demo1 command.
        /// </summary>
        /// <value>The Demo1 command.</value>
        public ICommand? Demo1Command
        {
            get { return GetPropertyValue(() => Demo1Command); }
            set { SetPropertyValue(() => Demo1Command, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Demo1 command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Demo1 command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void CanExecuteDemo1Command(object? sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the Demo1 command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments</param>
        public void ExecuteDemo1Command(object? sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessageAsync(new NavigateToDemo1View());
        }

        #endregion

        #region Property DemoPages

        /// <summary>
        /// Gets or sets the PageLinks.
        /// </summary>
        /// <value>The PageLinks.</value>
        public List<PageLink>? DemoPages
        {
            get { return GetPropertyValue(() => DemoPages); }
            set { SetPropertyValue(() => DemoPages, value); }
        }

        #endregion

    }
}
