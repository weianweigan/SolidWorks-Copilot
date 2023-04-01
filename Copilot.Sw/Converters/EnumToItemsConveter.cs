using System;
using System.Globalization;
using System.Windows.Data;

namespace Copilot.Sw.Converters;

public sealed class EnumToItemsConveter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum @enum)
        {
            return Enum.GetNames(value.GetType());
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
