using System.Globalization;

using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Myth.Avalonia.Controls.Converters
{
	public class EnumNotEqualsConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return false;

			return !value.Equals(parameter);
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> BindingOperations.DoNothing;
	}
}
