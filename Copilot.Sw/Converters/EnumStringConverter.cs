using System;
using System.Globalization;
using System.Windows.Data;

namespace Copilot.Sw.Converters;

public sealed class EnumStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum @enum)
        {
            return value.ToString();
        }

        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value as string;
        if (targetType?.IsEnum == true && !string.IsNullOrEmpty(str))
        {
            return Enum.Parse(targetType, str);
        }
        return null;
    }
}
