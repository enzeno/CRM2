using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CRM2.Desktop.Features.Quotations;

public class StatusToColorConverter : IValueConverter
{
    public static readonly StatusToColorConverter Instance = new();

    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status.StartsWith("Error", StringComparison.OrdinalIgnoreCase)
                ? Brushes.Red
                : Brushes.Green;
        }
        return Brushes.Black;
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StringNotEmptyConverter : IValueConverter
{
    public static readonly StringNotEmptyConverter Instance = new();

    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str);
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 