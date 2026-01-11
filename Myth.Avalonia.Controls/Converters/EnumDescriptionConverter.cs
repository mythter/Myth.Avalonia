using System.Globalization;

using Avalonia.Data.Converters;

using Myth.Avalonia.Controls.Extensions;

namespace Myth.Avalonia.Controls.Converters
{
	public class EnumDescriptionConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> value is Enum e ? e.GetDescription() : value?.ToString();

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
