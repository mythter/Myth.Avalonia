using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;

using Myth.Avalonia.Controls.Enums;
using Myth.Avalonia.Controls.Options;

namespace Myth.Avalonia.Controls
{
	public static class MessageBox
	{
		#region Public Show Methods

		public static void Show(MessageBoxOptions options)
		{
			ShowInternal(options);
		}

		public static void Show(
			string message,
			string title,
			MessageBoxButtons buttons = MessageBoxButtons.Ok,
			MessageBoxIcon icon = MessageBoxIcon.Info)
		{
			ShowInternal(message, title, buttons, icon);
		}

		public static Task<MessageBoxResult> ShowDialog(Window owner, MessageBoxOptions options)
		{
			return ShowDialogInternal(owner, options);
		}

		public static Task<MessageBoxResult> ShowDialog(
			Window owner,
			string message,
			string title,
			MessageBoxButtons buttons = MessageBoxButtons.Ok,
			MessageBoxIcon icon = MessageBoxIcon.Info)
		{
			ArgumentNullException.ThrowIfNull(owner);

			return ShowDialogInternal(owner, message, title, buttons, icon);
		}

		public static Task<MessageBoxResult> ShowDialog(
			string message,
			string title,
			MessageBoxButtons buttons = MessageBoxButtons.Ok,
			MessageBoxIcon icon = MessageBoxIcon.Info)
		{
			var owner = GetMainWindow();

			return ShowDialogInternal(owner, message, title, buttons, icon);
		}

		#region Convenient Methods

		public static void ShowInfo(
			string message,
			string title = "Info")
		{
			ShowInternal(message, title, MessageBoxButtons.Ok, MessageBoxIcon.Info);
		}

		public static void ShowWarning(
			string message,
			string title = "Warning")
		{
			ShowInternal(message, title, MessageBoxButtons.Ok, MessageBoxIcon.Warning);
		}

		public static void ShowError(
			string message,
			string title = "Error")
		{
			ShowInternal(message, title, MessageBoxButtons.Ok, MessageBoxIcon.Error);
		}

		public static void ShowQuestion(
			string message,
			string title = "Question")
		{
			ShowInternal(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}

		public static void ShowSuccess(
			string message,
			string title = "Success")
		{
			ShowInternal(message, title, MessageBoxButtons.Ok, MessageBoxIcon.Info);
		}

		#endregion

		#endregion

		#region Helper Methods

		private static MessageBoxOptions CreateOptions(
			string message,
			string title,
			MessageBoxButtons buttons = MessageBoxButtons.Ok,
			MessageBoxIcon icon = MessageBoxIcon.Info)
		{
			return new MessageBoxOptions
			{
				Title = title,
				Message = message,
				Buttons = buttons,
				Icon = icon
			};
		}

		private static void ShowInternal(
			string message,
			string title,
			MessageBoxButtons buttons = MessageBoxButtons.Ok,
			MessageBoxIcon icon = MessageBoxIcon.Info)
		{
			var options = CreateOptions(message, title, buttons, icon);

			ShowInternal(options);
		}

		private static void ShowInternal(MessageBoxOptions options)
		{
			ArgumentNullException.ThrowIfNull(options);

			// Ensure we're on UI thread
			if (!Dispatcher.UIThread.CheckAccess())
			{
				Dispatcher.UIThread.Post(() => ShowInternal(options));
			}

			Window window = PrepareMessageBox(options);

			window.Show();
		}

		private static Task<MessageBoxResult> ShowDialogInternal(
			Window owner,
			string message,
			string title,
			MessageBoxButtons buttons = MessageBoxButtons.Ok,
			MessageBoxIcon icon = MessageBoxIcon.Info)
		{
			var options = CreateOptions(message, title, buttons, icon);

			return ShowDialogInternal(owner, options);
		}

		private static async Task<MessageBoxResult> ShowDialogInternal(Window owner, MessageBoxOptions options)
		{
			ArgumentNullException.ThrowIfNull(options);

			// Ensure we're on UI thread
			if (!Dispatcher.UIThread.CheckAccess())
			{
				return await Dispatcher.UIThread.InvokeAsync(() => ShowDialogInternal(owner, options));
			}

			Window window = PrepareMessageBox(options);

			var result = await window.ShowDialog<MessageBoxResult>(owner);

			return result;
		}

		private static Window PrepareMessageBox(MessageBoxOptions options)
		{
			var window = CreateWindow(options);
			var content = CreateContent(options);

			window.Content = content;

			// Enable window dragging by header
			content.HeaderPointerPressed += (s, e) =>
			{
				if (e.GetCurrentPoint(content).Properties.IsLeftButtonPressed)
				{
					window.BeginMoveDrag(e);
				}
			};

			// Setup window closing on result
			var resultTask = content.ResultTask;
			_ = resultTask.ContinueWith(t =>
			{
				Dispatcher.UIThread.Post(() =>
				{
					if (window.IsVisible)
					{
						window.Close(t.Result);
					}
				});
			}, TaskScheduler.Default);

			return window;
		}

		private static Window CreateWindow(MessageBoxOptions options)
		{
			var window = new Window
			{
				Width = options.Width,
				MinHeight = options.MinHeight,
				MaxHeight = options.MaxHeight,
				WindowStartupLocation = options.StartupLocation,
				SystemDecorations = SystemDecorations.None,
				SizeToContent = SizeToContent.WidthAndHeight,
				TransparencyLevelHint = [WindowTransparencyLevel.Transparent],
				Topmost = options.Topmost,
				Focusable = true,
				Background = Brushes.Transparent
			};

			return window;
		}

		private static MessageBoxContent CreateContent(MessageBoxOptions options)
		{
			var content = new MessageBoxContent
			{
				Title = options.Title,
				Message = options.Message,
				Content = options.MessageContent,
				Icon = options.Icon,
				IsTextSelectable = options.IsTextSelectable,
				CornerRadius = new(options.CornerRadius),
				Buttons = options.Buttons,
				PrimaryButtons = options.PrimaryButtons,
				ButtonsPosition = options.ButtonsPosition,
				OkButtonText = options.OkButtonText,
				CancelButtonText = options.CancelButtonText,
				YesButtonText = options.YesButtonText,
				NoButtonText = options.NoButtonText,
				CanMinimize = options.CanMinimize
			};

			if (options.MessageTemplate is not null)
				content.MessageTemplate = options.MessageTemplate;

			return content;
		}

		private static Window GetMainWindow()
		{
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				return desktop.MainWindow ?? throw new InvalidOperationException("Cannot open dialog: Main window is null.");
			}

			throw new NotSupportedException($"Application lifetime is not supported: {Application.Current?.ApplicationLifetime?.GetType().Name}.");
		}

		#endregion
	}
}
