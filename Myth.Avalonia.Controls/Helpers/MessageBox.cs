using Avalonia.Controls;

using Myth.Avalonia.Controls.Enums;
using Myth.Avalonia.Controls.Options;
using Myth.Avalonia.Controls.Windows;

namespace Myth.Avalonia.Controls
{
	public static class MessageBox
	{
		public static void Show(MessageBoxOptions options)
		{
			var win = new MessageBoxWindow(options);

			win.Show();
		}

		public static Task<MessageBoxResult> ShowDialog(Window owner, MessageBoxOptions options)
		{
			var win = new MessageBoxWindow(options);

			win.Show();

			return win.ShowDialog<MessageBoxResult>(owner);
		}
	}
}
