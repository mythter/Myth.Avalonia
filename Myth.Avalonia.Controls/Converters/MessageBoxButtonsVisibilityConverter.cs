using System.Globalization;

using Avalonia.Data.Converters;

using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.Converters
{
	public class MessageBoxButtonsVisibilityConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is MessageBoxButtons buttons && parameter is MessageBoxButtons flag)
			{
				return buttons.HasFlag(flag);
			}
			return false;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
