using System;
using ModernWpf;
using System.Threading.Tasks;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Core;

namespace TimePunch_WPF_StackedModern_Demo.ViewModels
{
    public class SettingsViewModel : DemoViewModelBase
    {
        public override Task<bool> InitializePageAsync(object extraData)
        {
            if (DemoKernel.Instance!.Controller is StackedController stackedController)
            {
                SetPropertyValue(() => NavigationStyle, (int)stackedController.StackedMode);
                SetPropertyValue(() => FadeInDuration, stackedController.StackedFrame!.FadeInDuration);
                SetPropertyValue(() => FadeOutDuration, stackedController.StackedFrame!.FadeOutDuration);
            }

            var theme = ThemeManager.Current.ApplicationTheme;
            if (theme == ModernWpf.ApplicationTheme.Light)
                SetPropertyValue(() => ApplicationTheme, 1);
            else if (theme == ModernWpf.ApplicationTheme.Dark)
                SetPropertyValue(() => ApplicationTheme, 2);
            else
                SetPropertyValue(() => ApplicationTheme, 0);

            return base.InitializePageAsync(extraData);
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
                    if (DemoKernel.Instance.Controller is StackedController stackedController)
                        stackedController.StackedMode = (StackedMode)value;
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
                {
                    if (DemoKernel.Instance!.Controller is StackedController stackedController)
                        stackedController.StackedFrame!.FadeInDuration = value;
                }
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
                {
                    if (DemoKernel.Instance!.Controller is StackedController stackedController)
                        stackedController.StackedFrame!.FadeOutDuration = value;
                }
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
                    ApplicationTheme? theme = value switch
                    {
                        0 => null,
                        1 => ModernWpf.ApplicationTheme.Light,
                        2 => ModernWpf.ApplicationTheme.Dark,
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                    };
                    ThemeManager.Current.ApplicationTheme = theme;
                }
            }
        }

        #endregion
    }
}
