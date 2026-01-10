using System.Globalization;

using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;

using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.Converters
{
	public class MessageBoxIconToColorConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is MessageBoxIcon icon)
			{
				if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
				{
					return icon switch
					{
						MessageBoxIcon.Info => new SolidColorBrush(Color.Parse("#CABCF6")),
						MessageBoxIcon.Warning => new SolidColorBrush(Color.Parse("#FFD800")),
						MessageBoxIcon.Error => new SolidColorBrush(Color.Parse("#C01B1B")),
						MessageBoxIcon.Question => new SolidColorBrush(Color.Parse("#2F98D8")),
						_ => Brushes.White
					};
				}
				else
				{
					return icon switch
					{
						MessageBoxIcon.Info => new SolidColorBrush(Color.Parse("#FF82799E")),
						MessageBoxIcon.Warning => new SolidColorBrush(Color.Parse("#FFD800")),
						MessageBoxIcon.Error => new SolidColorBrush(Color.Parse("#C01B1B")),
						MessageBoxIcon.Question => new SolidColorBrush(Color.Parse("#2F98D8")),
						_ => Brushes.Black
					};
				}
			}

			return Brushes.Black;
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
