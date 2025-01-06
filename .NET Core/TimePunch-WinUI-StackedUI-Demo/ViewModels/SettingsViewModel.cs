using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using TimePunch_WinUI_StackedUI_Demo.Core;
using TimePunch.MVVM.Controller;
using TimePunch.StackedUI.Controller;
using System.Collections.ObjectModel;
using TimePunch_WinUI_StackedUI_Demo.Models;
using TimePunch.StackedUI.Controls;

namespace TimePunch_WinUI_StackedUI_Demo.ViewModels
{
    public class SettingsViewModel : DemoViewModelBase
    {
        public override Task<bool> InitializePageAsync(object extraData, DispatcherQueue dispatcher)
        {
            if (DemoKernel.Instance.Controller is StackedController stackedController)
            {
                SetPropertyValue(() => NavigationStyle, (int)stackedController.StackedMode);
                SetPropertyValue(() => FadeInDuration, stackedController.StackedFrame!.FadeInDuration);
                SetPropertyValue(() => FadeOutDuration, stackedController.StackedFrame!.FadeOutDuration);
            }

            var windowContent = DemoKernel.Instance.MainWindow.Content as FrameworkElement;
            if (windowContent != null)
                SetPropertyValue(() => ApplicationTheme, (int)windowContent.RequestedTheme);

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
                    if (DemoKernel.Instance.Controller is StackedController stackedController)
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
                    if (DemoKernel.Instance.Controller is StackedController stackedController)
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
                    DemoKernel.Instance.MainWindow.DpiDecorator.Scale = value.Value/100.0;
                }
            }
        }

        #endregion
    }
}
