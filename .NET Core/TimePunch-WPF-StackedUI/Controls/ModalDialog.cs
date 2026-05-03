using ModernWpf.Controls;

namespace TimePunch.StackedUI.Controls
{
    public class ModalDialog : ContentDialog
    {
        private bool? dialogResult;

        /// <summary>
        /// Initializes a new instance of the modal dialog
        /// </summary>
        public ModalDialog()
        {
            Closing += OnClosing;
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
            args.Cancel = AttachedProperties.GetDialogResult(sender) == null;
        }

    }
}
