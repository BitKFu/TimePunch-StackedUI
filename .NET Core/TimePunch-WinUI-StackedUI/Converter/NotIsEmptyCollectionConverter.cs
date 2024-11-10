using System.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TimePunch.StackedUI.Converter
{
    public class NotIsEmptyCollectionConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is IList enumerable)
                return enumerable.Count>0 ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException("ConvertBack of NotIsEmptyCollectionConverter not implemented.");
        }

        #endregion
    }
}
