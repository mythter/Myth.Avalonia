using System.Globalization;

using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;

using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.Converters
{
	public class MessageBoxPrimaryButtonsConverter : IValueConverter
	{
		private static IBrush? _defaultButtonBrush;

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is MessageBoxButtons buttons && parameter is MessageBoxButtons flag && buttons.HasFlag(flag) &&
				Application.Current?.TryGetResource("MessageBoxPrimaryButtonBackground", Application.Current.ActualThemeVariant, out var resource) == true && resource is IBrush accentBrush)
			{
				return accentBrush;
			}

			return GetDefaultBrush();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static IBrush? GetDefaultBrush()
		{
			if (_defaultButtonBrush is null &&
				Application.Current?.TryGetResource("MessageBoxButtonBackground", Application.Current.ActualThemeVariant, out var resource) == true &&
				resource is IBrush brush)
			{
				_defaultButtonBrush = brush;
			}

			return _defaultButtonBrush;
		}
	}
}
