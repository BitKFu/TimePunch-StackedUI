using System.Windows;

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
        GridLength GetPageWidth(string pageName);
    }
}
