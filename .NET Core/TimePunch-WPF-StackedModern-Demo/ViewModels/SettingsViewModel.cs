using System;
using ModernWpf;
using System.Threading.Tasks;
using TimePunch_WPF_StackedModern_Demo.Models;
using TimePunch.StackedUI.Controller;
using TimePunch.StackedUI.Demo.Core;
using System.Collections.ObjectModel;

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

            ScaleModes =
            [
                new Resource<int>("Extra small size", 60),
                new Resource<int>("Small size", 80),
                new Resource<int>("Normal size", 100)
            ];

            if (DemoKernel.Instance.MainWindow.DpiDecorator.Scale == 0.6)
                SelectedScaleMode = ScaleModes[0];
            else if (DemoKernel.Instance.MainWindow.DpiDecorator.Scale == 0.8)
                SelectedScaleMode = ScaleModes[1];
            else
                SelectedScaleMode = ScaleModes[2];

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

        #region Property ScaleModes

        /// <summary>
        /// Gets or sets the ScaleModes.
        /// </summary>
        /// <value>The ScaleModes.</value>
        public ObservableCollection<Resource<int>> ScaleModes
        {
            get { return GetPropertyValue(() => ScaleModes); }
            set { SetPropertyValue(() => ScaleModes, value); }
        }

        #endregion

        #region Property SelectedScaleMode

        /// <summary>
        /// Gets or sets the SelectedScaleMode.
        /// </summary>
        /// <value>The SelectedScaleMode.</value>
        public Resource<int> SelectedScaleMode
        {
            get { return GetPropertyValue(() => SelectedScaleMode); }
            set
            {
                if (SetPropertyValue(() => SelectedScaleMode, value))
                {
                    DemoKernel.Instance.MainWindow.DpiDecorator.Scale = value.Value / 100.0;
                }
            }
        }

        #endregion
    }
}
