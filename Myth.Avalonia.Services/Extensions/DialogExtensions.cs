using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Templates;
using Avalonia.Platform.Storage;
using Myth.Avalonia.Services.Abstractions;

namespace Myth.Avalonia.Services.Extensions;

public static class DialogExtensions
{
	#region Public Methods

	/// <summary>
	/// Shows an open file dialog for a registered context
	/// </summary>
	/// <param name="context">The context</param>
	/// <param name="title">The dialog title</param>
	/// <param name="fileTypeFilter">The dialog file type filter</param>
	/// <returns>Selected file path</returns>
	/// <exception cref="ArgumentNullException">if context is null</exception>
	public static async Task<string?> ShowOpenFileDialogAsync(this IDialogContext? context,
		string? title = null,
		Dictionary<string, string[]>? fileTypeFilter = null)
	{
		var files = await ShowOpenFileDialogAsync(context, title, fileTypeFilter, selectMany: false);

		return files.FirstOrDefault();
	}

	/// <summary>
	/// Shows an open file dialog for a registered context
	/// </summary>
	/// <param name="context">The context</param>
	/// <param name="title">The dialog title</param>
	/// <param name="fileTypeFilter">The dialog file type filter</param>
	/// <returns>An array of selected file paths</returns>
	/// <exception cref="ArgumentNullException">if context is null</exception>
	public static Task<List<string>> ShowOpenFilesDialogAsync(this IDialogContext? context,
		string? title = null,
		Dictionary<string, string[]>? fileTypeFilter = null)
	{
		return ShowOpenFileDialogAsync(context, title, fileTypeFilter, selectMany: true);
	}

	/// <summary>
	/// Shows a save file dialog for a registered context
	/// </summary>
	/// <param name="context">The context</param>
	/// <param name="title">The dialog title</param>
	/// <param name="fileTypeFilter">The dialog file type filter</param>
	/// <returns>An array of selected file paths</returns>
	/// <exception cref="ArgumentNullException">if context is null</exception>
	public static async Task<string?> ShowSaveFileDialogAsync(this IDialogContext? context,
		string? title = null,
		string? suggestedFileName = null,
		Dictionary<string, string[]>? fileTypeFilter = null)
	{
		ArgumentNullException.ThrowIfNull(context);

		// lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
		var topLevel = DialogManager.GetTopLevelForContext(context)
			?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

		var fileChoices = new List<FilePickerFileType>();

		foreach (var filter in fileTypeFilter ?? [])
		{
			fileChoices.Add(new FilePickerFileType(filter.Key)
			{
				Patterns = filter.Value.Length == 0 ? ["*"] : filter.Value
			});
		}

		var options = new FilePickerSaveOptions
		{
			Title = title ?? "Save file",
			SuggestedFileName = suggestedFileName,
			FileTypeChoices = fileTypeFilter is null ? null : fileChoices
		};

		var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);

		return file?.TryGetLocalPath() ?? file?.Name;
	}

	/// <summary>
	/// Shows a dialog window for a given context
	/// </summary>
	/// <param name="context">The context to use</param>
	/// <param name="windowTitle">The dialog's window title</param>
	/// <param name="content">The content to show</param>
	/// <param name="contentTemplate">Optional: An <see cref="IDataTemplate"/> to represnet the <see cref="content"/></param>
	/// <typeparam name="T">The expected type to return</typeparam>
	/// <returns>The result or null if dialog was canceled</returns>
	/// <exception cref="InvalidOperationException">The dialog window can only be shown if the app is a desktop app.</exception>
	public static async Task<T?> ShowDialogWindow<T>(this IDialogContext? context,
		string windowTitle,
		object content,
		IDataTemplate? contentTemplate = null)
	{
		ArgumentNullException.ThrowIfNull(context);

		// Get the owner window. If it is null, throw an exception
		var ownerWindow = DialogManager.GetTopLevelForContext(context) as Window
			?? throw new InvalidOperationException("The method ShowDialogWindow can only be used on a Window");

		var dialog = new Window()
		{
			Title = windowTitle,
			Content = content,
			ContentTemplate = contentTemplate,
			SizeToContent = SizeToContent.WidthAndHeight,
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
		};

		return await dialog.ShowDialog<T>(ownerWindow);
	}

	/// <summary>
	/// Closes a dialog window with the given result
	/// </summary>
	/// <param name="context">The context to resolve the window</param>
	/// <param name="result">The result to return</param>
	/// <exception cref="InvalidOperationException">If the <see cref="TopLevel"/> is not a <see cref="Window"/></exception>
	public static void ReturnResultFromDialogWindow(this IDialogContext? context, object? result)
	{
		ArgumentNullException.ThrowIfNull(context);

		var dialogWindow = DialogManager.GetTopLevelForContext(context) as Window
			?? throw new InvalidOperationException("The method ReturnResultFromDialogWindow can only be used on a Window");

		dialogWindow.Close(result);
	}

	/// <summary>
	/// Adds a notification to the <see cref="WindowNotificationManager"/>.
	/// </summary>
	/// <param name="context">The context to resolve the WindowNotificationManager</param>
	/// <param name="title">The title of the notification</param>
	/// <param name="message">The message of the notification</param>
	/// <param name="notificationType">The see <see cref="NotificationType"/> to use</param>
	/// <param name="notificationPosition">The see <see cref="NotificationPosition"/> to use</param>
	/// <param name="expiration">The expiration time of the notification</param>
	/// <exception cref="InvalidOperationException">If no WindowNotificationManager was found for the given context</exception>
	public static void ShowNotificationMessage(this IDialogContext? context,
		string title,
		string message,
		NotificationType notificationType = NotificationType.Information,
		NotificationPosition? notificationPosition = null,
		TimeSpan? expiration = null)
	{
		ShowNotificationMessage(
			context,
			new Notification(title, message, notificationType, expiration ?? TimeSpan.FromSeconds(3)),
			notificationPosition);
	}

	/// <summary>
	/// Adds a given <see cref="Notification"/> to the <see cref="WindowNotificationManager"/>.
	/// </summary>
	/// <param name="context">The context to resolve the WindowNotificationManager</param>
	/// <param name="notification">The notification to display</param>
	/// <param name="notificationPosition">The notification position</param>
	/// <exception cref="InvalidOperationException">If no WindowNotificationManager was found for the given context</exception>
	public static void ShowNotificationMessage(this IDialogContext? context, Notification notification, NotificationPosition? notificationPosition)
	{
		ArgumentNullException.ThrowIfNull(context);

		var notificationManager = DialogManager.GetVisualForContext(context) as WindowNotificationManager
			?? throw new InvalidOperationException("The method ShowNotificationMessage must be used on a WindowNotificationManager");

		if (notificationPosition is not null)
		{
			notificationManager.Position = notificationPosition.Value;
		}

		notificationManager.Show(notification);
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Shows an open file dialog for a registered context
	/// </summary>
	/// <param name="context">The context</param>
	/// <param name="title">The dialog title</param>
	/// <param name="fileTypeFilter">The dialog file type filter</param>
	/// <param name="selectMany">Is selecting many files allowed?</param>
	/// <returns>An array of file names</returns>
	/// <exception cref="ArgumentNullException">if context was null</exception>
	private static async Task<List<string>> ShowOpenFileDialogAsync(
		IDialogContext? context,
		string? title,
		Dictionary<string, string[]>? fileTypeFilter,
		bool selectMany)
	{
		ArgumentNullException.ThrowIfNull(context);

		// lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
		var topLevel = DialogManager.GetTopLevelForContext(context)
			?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

		var fileFilters = new List<FilePickerFileType>();

		foreach (var filter in fileTypeFilter ?? [])
		{
			fileFilters.Add(new FilePickerFileType(filter.Key)
			{
				Patterns = filter.Value.Length == 0 ? ["*"] : filter.Value
			});
		}

		var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(
			new FilePickerOpenOptions()
			{
				AllowMultiple = selectMany,
				FileTypeFilter = fileTypeFilter is null ? null : fileFilters,
				Title = title ?? "Select file(s)"
			});

		return [.. storageFiles.Select(s => s.TryGetLocalPath() ?? s.Name)];
	}

	#endregion
}
