using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Myth.Avalonia.Themes
{
	public class MythControls : Styles
	{
		public MythControls()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
