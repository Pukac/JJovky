using System.Globalization;

namespace JJPoems.Converters;

public class HasTextConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return !string.IsNullOrWhiteSpace(value?.ToString());
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value ?? string.Empty;
	}
}
