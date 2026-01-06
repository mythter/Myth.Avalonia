using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.Options
{
	public class MessageBoxOptions
	{
		public string? Title { get; set; }

		public string? Message { get; set; }

		public MessageBoxIcon Icon { get; set; }

		public bool IsTextSelectable { get; set; } = false;
	}
}
