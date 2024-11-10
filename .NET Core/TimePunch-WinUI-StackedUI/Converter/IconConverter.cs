using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace TimePunch.StackedUI.Converter
{
    public class IconConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object? parameter, string language)
        {
            var icon = value.ToString();
            if (icon == null)
                return null;

            IconElement? result = null;
            if (icon.Contains("://"))
                result = new BitmapIcon() { UriSource = new Uri(icon) };
            else
            {
                if (icon.StartsWith("m ", StringComparison.InvariantCultureIgnoreCase))
                    result = new PathIcon() { Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), icon) };
                else
                {
                    if (parameter != null)
                    {
                        if (!Application.Current.Resources.TryGetValue(parameter, out var fontFamily))
                            Application.Current.Resources.TryGetValue("SymbolThemeFontFamily", out fontFamily);

                        if (fontFamily != null)
                            result = new FontIcon()
                            {
                                FontFamily = (FontFamily)fontFamily,
                                Glyph = icon
                            };
                    }
                    else
                        result = new SymbolIcon((Symbol)Enum.Parse(typeof(Symbol), icon));
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
