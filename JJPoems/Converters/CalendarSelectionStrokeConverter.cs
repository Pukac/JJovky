using System.Globalization;

namespace JJPoems.Converters;

public class CalendarSelectionStrokeConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is true ? Color.FromArgb("#A8825B") : Color.FromArgb("#D8C8B3");
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
