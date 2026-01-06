using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Myth.Avalonia.Themes
{
	public class MythStyles : Styles
	{
		public MythStyles()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
