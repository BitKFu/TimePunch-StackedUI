using Microsoft.UI.Xaml;

namespace TimePunch.StackedUI.Model
{
    public interface IPagePersister
    {
        /// <summary>
        /// Save the page width
        /// </summary>
        void SavePageWidth(string pageName, GridLength pageWidth);

        /// <summary>
        /// Load the page width
        /// </summary>
        double GetPageWidth(string pageName);
    }
}
