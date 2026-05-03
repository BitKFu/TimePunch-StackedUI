using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.StackedUI.ViewModels;

namespace TimePunch.StackedUI.Controls
{
    public class ModalDialog : ContentDialog
    {
        private bool? dialogResult;
        private StackedViewModelBase? viewModel;

        /// <summary>
        /// Initializes a new instance of the modal dialog
        /// </summary>
        public ModalDialog()
        {
            Closing += OnClosing;
            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// Gets or sets the dialog result
        /// </summary>
        public bool? DialogResult
        {
            get => dialogResult;
            set
            {
                dialogResult = value;
                if (dialogResult != null)
                    Hide();
            }
        }

        /// <summary>
        /// Called when the dialog gets closed
        /// </summary>
        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = AttachedProperties.GetDialogResult(sender) == null && DialogResult == null;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (viewModel != null)
                viewModel.PropertyChanged -= OnViewModelPropertyChanged;

            viewModel = args.NewValue as StackedViewModelBase;

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                ApplyDialogResult(viewModel.DialogResult);
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StackedViewModelBase.DialogResult) && sender is StackedViewModelBase stackedViewModel)
                ApplyDialogResult(stackedViewModel.DialogResult);
        }

        private void ApplyDialogResult(bool? value)
        {
            if (value != null && dialogResult == null)
                DialogResult = value;
        }

    }
}
