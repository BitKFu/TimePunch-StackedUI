using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch.StackedUI.Controller;

namespace TimePunch_WinUI_StackedUI_Demo.ViewModels
{
    public class SettingsViewModel : DemoViewModelBase
    {
        public override Task<bool> InitializePageAsync(object extraData, DispatcherQueue dispatcher)
        {
            SetPropertyValue(() => NavigationStyle, (int)DemoKernel.Instance.Controller.StackedMode);
            SetPropertyValue(() => FadeInDuration,  DemoKernel.Instance.Controller.StackedFrame!.FadeInDuration);
            SetPropertyValue(() => FadeOutDuration, DemoKernel.Instance.Controller.StackedFrame!.FadeOutDuration);

            var windowContent = DemoKernel.Instance.MainWindow.Content as FrameworkElement;
            if (windowContent != null)
                SetPropertyValue(() => ApplicationTheme, (int)windowContent.RequestedTheme);

            return base.InitializePageAsync(extraData, dispatcher);
        }

        #region Property NavigationStyle

        /// <summary>
        /// Gets or sets the NavigationStyle.
        /// </summary>
        /// <value>The NavigationStyle.</value>
        public int NavigationStyle
        {
            get { return GetPropertyValue(() => NavigationStyle); }
            set
            {
                if (SetPropertyValue(() => NavigationStyle, value))
                {
                    DemoKernel.Instance.Controller.StackedMode = (StackedMode)value;
                }
            }
        }

        #endregion

        #region Property FadeInDuration

        /// <summary>
        /// Gets or sets the FadeInDuration.
        /// </summary>
        /// <value>The FadeInDuration.</value>
        public int FadeInDuration
        {
            get { return GetPropertyValue(() => FadeInDuration); }
            set
            {
                if (SetPropertyValue(() => FadeInDuration, value))
                    DemoKernel.Instance.Controller.StackedFrame!.FadeInDuration = value;
            }
        }

        #endregion

        #region Property FadeOutDuration

        /// <summary>
        /// Gets or sets the FadeOutDuration.
        /// </summary>
        /// <value>The FadeOutDuration.</value>
        public int FadeOutDuration
        {
            get { return GetPropertyValue(() => FadeOutDuration); }
            set
            {
                if (SetPropertyValue(() => FadeOutDuration, value))
                    DemoKernel.Instance.Controller.StackedFrame!.FadeOutDuration = value;
            }
        }

        #endregion

        #region Property ApplicationTheme

        /// <summary>
        /// Gets or sets the ApplicationTheme.
        /// </summary>
        /// <value>The ApplicationTheme.</value>
        public int ApplicationTheme
        {
            get { return GetPropertyValue(() => ApplicationTheme); }
            set
            {
                if (SetPropertyValue(() => ApplicationTheme, value))
                {
                    var windowContent = DemoKernel.Instance.MainWindow.Content as FrameworkElement;
                    if (windowContent == null)
                        return;

                    windowContent.RequestedTheme = value switch
                    {
                        0 => ElementTheme.Default,
                        1 => ElementTheme.Light,
                        2 => ElementTheme.Dark,
                        _ => windowContent.RequestedTheme
                    };
                }
            }
        }

        #endregion
    }
}
