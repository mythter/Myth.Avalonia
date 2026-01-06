using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using Myth.Avalonia.Controls.Enums;
using Myth.Avalonia.Controls.Options;

namespace Myth.Avalonia.Controls;

public partial class MessageBoxControl : UserControl
{
	#region Public Properties

	public Window? Owner { get; set; }

	#endregion

	#region Events

	public event EventHandler<PointerPressedEventArgs>? HeaderPointerPressed;

	public event EventHandler<MessageBoxResult>? Close;

	#endregion

	#region IsTextSelectable

	public static readonly StyledProperty<bool> IsTextSelectableProperty =
		AvaloniaProperty.Register<MessageBoxControl, bool>(nameof(IsTextSelectable));

	public bool IsTextSelectable
	{
		get => GetValue(IsTextSelectableProperty);
		set => SetValue(IsTextSelectableProperty, value);
	}

	#endregion

	#region Title

	public static readonly StyledProperty<string?> TitleProperty =
		AvaloniaProperty.Register<MessageBoxControl, string?>(nameof(Title));

	public string? Title
	{
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	#endregion

	#region Message

	public static readonly StyledProperty<string?> MessageProperty =
		AvaloniaProperty.Register<MessageBoxControl, string?>(nameof(Message));

	public string? Message
	{
		get => GetValue(MessageProperty);
		set => SetValue(MessageProperty, value);
	}

	#endregion

	#region Icon

	public static readonly StyledProperty<MessageBoxIcon> IconProperty =
		AvaloniaProperty.Register<MessageBoxControl, MessageBoxIcon>(nameof(Icon));

	public MessageBoxIcon Icon
	{
		get => GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	#endregion

	#region Options

	private MessageBoxOptions? _options;

	public MessageBoxOptions? Options
	{
		get => _options;
		set
		{
			_options = value;
			ApplyOptions();
		}
	}

	#endregion

	public MessageBoxControl()
	{
		InitializeComponent();
	}

	private void ApplyOptions()
	{
		if (Options == null) return;

		IsTextSelectable = Options.IsTextSelectable;
		Icon = Options.Icon;
		Title = Options.Title;
		Message = Options.Message;
	}

	#region Event Handlers

	private void OnOkClick(object? sender, RoutedEventArgs e)
	{
		Close?.Invoke(this, MessageBoxResult.Ok);
	}

	private void OnCloseClick(object? sender, RoutedEventArgs e)
	{
		Close?.Invoke(this, MessageBoxResult.Cancel);
	}

	private void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		HeaderPointerPressed?.Invoke(this, e);
	}

	#endregion
}