using Avalonia.Controls;
using Avalonia.Controls.Templates;

using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.Options
{
	public class MessageBoxOptions
	{
		public string? Title { get; set; }

		public string? Message { get; set; }

		public MessageBoxIcon Icon { get; set; }

		public bool IsTextSelectable { get; set; } = false;

		public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.Ok;
		public MessageBoxButtons PrimaryButtons { get; set; } = MessageBoxButtons.None;

		public MessageBoxButtonsPosition ButtonsPosition { get; set; } = MessageBoxButtonsPosition.Center;

		public IDataTemplate? MessageTemplate { get; set; }

		public object? MessageContent { get; set; }

		public double Width { get; set; } = 450;
		public double MinHeight { get; set; } = 200;
		public double MaxHeight { get; set; } = 600;
		public bool CanMinimize { get; set; }
		public bool Topmost { get; set; }
		public WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.CenterScreen;

		public int CornerRadius { get; set; } = 5;

		public string OkButtonText { get; set; } = "OK";
		public string CancelButtonText { get; set; } = "Cancel";
		public string YesButtonText { get; set; } = "Yes";
		public string NoButtonText { get; set; } = "No";
	}
}
