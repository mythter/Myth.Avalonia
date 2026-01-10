using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

using Myth.Avalonia.Controls.Commands;
using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls;

public class MessageBoxContent : ContentControl
{
	private Border? _headerBorder;

	#region HeaderBackground

	public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
		AvaloniaProperty.Register<MessageBoxContent, IBrush?>(nameof(HeaderBackground));

	public IBrush? HeaderBackground
	{
		get => this.GetValue(HeaderBackgroundProperty);
		set => SetValue(HeaderBackgroundProperty, value);
	}
	#endregion

	#region HeaderForeground

	public static readonly StyledProperty<IBrush?> HeaderForegroundProperty =
		AvaloniaProperty.Register<MessageBoxContent, IBrush?>(nameof(HeaderForeground));

	public IBrush? HeaderForeground
	{
		get => this.GetValue(HeaderForegroundProperty);
		set => SetValue(HeaderForegroundProperty, value);
	}

	#endregion

	#region HeaderCornerRadius

	private CornerRadius _headerCornerRadius;

	public static readonly DirectProperty<MessageBoxContent, CornerRadius> HeaderCornerRadiusProperty =
		AvaloniaProperty.RegisterDirect<MessageBoxContent, CornerRadius>(
			nameof(HeaderCornerRadius),
			o => o.HeaderCornerRadius);

	public CornerRadius HeaderCornerRadius
	{
		get => _headerCornerRadius;
		private set => SetAndRaise(HeaderCornerRadiusProperty, ref _headerCornerRadius, value);
	}

	#endregion

	#region Title

	public static readonly StyledProperty<string?> TitleProperty =
		AvaloniaProperty.Register<MessageBoxContent, string?>(nameof(Title));

	public string? Title
	{
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	#endregion

	#region Message

	public static readonly StyledProperty<object?> MessageProperty =
		AvaloniaProperty.Register<MessageBoxContent, object?>(nameof(Message));

	public object? Message
	{
		get => GetValue(MessageProperty);
		set => SetValue(MessageProperty, value);
	}

	#endregion

	#region ActualContent

	private object? _actualContent;

	public static readonly DirectProperty<MessageBoxContent, object?> ActualContentProperty =
		AvaloniaProperty.RegisterDirect<MessageBoxContent, object?>(
			nameof(ActualContent),
			o => o.ActualContent);

	/// <summary>
	/// Returns Content if setted, otherwise Message
	/// </summary>
	public object? ActualContent
	{
		get => _actualContent;
		private set => SetAndRaise(ActualContentProperty, ref _actualContent, value);
	}

	#endregion

	#region Icon

	public static readonly StyledProperty<MessageBoxIcon> IconProperty =
		AvaloniaProperty.Register<MessageBoxContent, MessageBoxIcon>(nameof(Icon));
	public MessageBoxIcon Icon
	{
		get => GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	#endregion

	#region Events

	#region HeaderPointerPressed

	public event EventHandler<PointerPressedEventArgs>? HeaderPointerPressed;

	#endregion

	#endregion

	#region IsTextSelectable

	public static readonly StyledProperty<bool> IsTextSelectableProperty =
		AvaloniaProperty.Register<MessageBoxContent, bool>(nameof(IsTextSelectable));

	public bool IsTextSelectable
	{
		get => GetValue(IsTextSelectableProperty);
		set => SetValue(IsTextSelectableProperty, value);
	}

	#endregion

	#region MessageTemplate

	public static readonly StyledProperty<IDataTemplate?> MessageTemplateProperty =
		AvaloniaProperty.Register<MessageBoxContent, IDataTemplate?>(nameof(MessageTemplate));

	public IDataTemplate? MessageTemplate
	{
		get => GetValue(MessageTemplateProperty);
		set => SetValue(MessageTemplateProperty, value);
	}

	#endregion

	#region Result

	private readonly TaskCompletionSource<MessageBoxResult>? _resultSource;

	public Task<MessageBoxResult> ResultTask => _resultSource?.Task ?? Task.FromResult(MessageBoxResult.Cancel);

	#endregion

	#region CanMinimize

	public static readonly StyledProperty<bool> CanMinimizeProperty =
		AvaloniaProperty.Register<MessageBoxContent, bool>(nameof(CanMinimize));

	public bool CanMinimize
	{
		get => GetValue(CanMinimizeProperty);
		set => SetValue(CanMinimizeProperty, value);
	}

	#endregion

	#region Commands

	#region OkCommand

	public static readonly StyledProperty<ICommand?> OkCommandProperty =
		AvaloniaProperty.Register<MessageBoxContent, ICommand?>(nameof(OkCommand));

	public ICommand? OkCommand
	{
		get => GetValue(OkCommandProperty);
		set => SetValue(OkCommandProperty, value);
	}

	#endregion

	#region CancelCommand

	public static readonly StyledProperty<ICommand?> CancelCommandProperty =
		AvaloniaProperty.Register<MessageBoxContent, ICommand?>(nameof(CancelCommand));

	public ICommand? CancelCommand
	{
		get => GetValue(CancelCommandProperty);
		set => SetValue(CancelCommandProperty, value);
	}

	#endregion

	#region CloseCommand

	public static readonly StyledProperty<ICommand?> CloseCommandProperty =
		AvaloniaProperty.Register<MessageBoxContent, ICommand?>(nameof(CloseCommand));

	public ICommand? CloseCommand
	{
		get => GetValue(CloseCommandProperty);
		set => SetValue(CloseCommandProperty, value);
	}

	#endregion

	#region YesCommand

	public static readonly StyledProperty<ICommand?> YesCommandProperty =
		AvaloniaProperty.Register<MessageBoxContent, ICommand?>(nameof(YesCommand));

	public ICommand? YesCommand
	{
		get => GetValue(YesCommandProperty);
		set => SetValue(YesCommandProperty, value);
	}

	#endregion

	#region NoCommand

	public static readonly StyledProperty<ICommand?> NoCommandProperty =
		AvaloniaProperty.Register<MessageBoxContent, ICommand?>(nameof(NoCommand));

	public ICommand? NoCommand
	{
		get => GetValue(NoCommandProperty);
		set => SetValue(NoCommandProperty, value);
	}

	#endregion

	#region MinimizeCommand

	public static readonly StyledProperty<ICommand?> MinimizeCommandProperty =
		AvaloniaProperty.Register<MessageBoxContent, ICommand?>(nameof(MinimizeCommand));

	public ICommand? MinimizeCommand
	{
		get => GetValue(MinimizeCommandProperty);
		private set => SetValue(MinimizeCommandProperty, value);
	}

	#endregion

	#endregion

	#region Buttons

	#region Buttons

	public static readonly StyledProperty<MessageBoxButtons> ButtonsProperty =
		AvaloniaProperty.Register<MessageBoxContent, MessageBoxButtons>(nameof(Buttons), MessageBoxButtons.Ok);

	public MessageBoxButtons Buttons
	{
		get => GetValue(ButtonsProperty);
		set => SetValue(ButtonsProperty, value);
	}

	#endregion

	#region PrimaryButtons

	public static readonly StyledProperty<MessageBoxButtons> PrimaryButtonsProperty =
		AvaloniaProperty.Register<MessageBoxContent, MessageBoxButtons>(nameof(PrimaryButtons), MessageBoxButtons.None);

	public MessageBoxButtons PrimaryButtons
	{
		get => GetValue(PrimaryButtonsProperty);
		set => SetValue(PrimaryButtonsProperty, value);
	}

	#endregion

	#region ButtonsPosition

	public static readonly StyledProperty<MessageBoxButtonsPosition> ButtonsPositionProperty =
		AvaloniaProperty.Register<MessageBoxContent, MessageBoxButtonsPosition>(nameof(ButtonsPosition), MessageBoxButtonsPosition.Center);

	public MessageBoxButtonsPosition ButtonsPosition
	{
		get => GetValue(ButtonsPositionProperty);
		set => SetValue(ButtonsPositionProperty, value);
	}

	#endregion

	#region OkButton

	public static readonly StyledProperty<string> OkButtonTextProperty =
		AvaloniaProperty.Register<MessageBoxContent, string>(nameof(OkButtonText), "OK");

	public string OkButtonText
	{
		get => GetValue(OkButtonTextProperty);
		set => SetValue(OkButtonTextProperty, value);
	}

	#endregion

	#region CancelButton

	public static readonly StyledProperty<string> CancelButtonTextProperty =
		AvaloniaProperty.Register<MessageBoxContent, string>(nameof(CancelButtonText), "Cancel");

	public string CancelButtonText
	{
		get => GetValue(CancelButtonTextProperty);
		set => SetValue(CancelButtonTextProperty, value);
	}

	#endregion

	#region YesButton

	public static readonly StyledProperty<string> YesButtonTextProperty =
		AvaloniaProperty.Register<MessageBoxContent, string>(nameof(YesButtonText), "Yes");

	public string YesButtonText
	{
		get => GetValue(YesButtonTextProperty);
		set => SetValue(YesButtonTextProperty, value);
	}

	#endregion

	#region NoButton

	public static readonly StyledProperty<string> NoButtonTextProperty =
		AvaloniaProperty.Register<MessageBoxContent, string>(nameof(NoButtonText), "No");

	public string NoButtonText
	{
		get => GetValue(NoButtonTextProperty);
		set => SetValue(NoButtonTextProperty, value);
	}

	#endregion

	#endregion

	public MessageBoxContent()
	{
		_resultSource = new TaskCompletionSource<MessageBoxResult>();
		InitializeDefaultCommands();
	}

	public void SetResult(MessageBoxResult result)
	{
		_resultSource?.TrySetResult(result);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		if (_headerBorder != null)
		{
			_headerBorder.PointerPressed -= OnHeaderBorderPointerPressed;
		}

		_headerBorder = e.NameScope.Find<Border>("PART_Header");

		if (_headerBorder != null)
		{
			_headerBorder.PointerPressed += OnHeaderBorderPointerPressed;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == CornerRadiusProperty)
		{
			var cr = CornerRadius;
			HeaderCornerRadius = new CornerRadius(cr.TopLeft, cr.TopRight, 0, 0);
		}
		else if (change.Property == ContentProperty || change.Property == MessageProperty)
		{
			UpdateActualContent();
		}
	}

	private void InitializeDefaultCommands()
	{
		OkCommand = new RelayCommand(() => SetResult(MessageBoxResult.Ok));
		CancelCommand = new RelayCommand(() => SetResult(MessageBoxResult.Cancel));
		YesCommand = new RelayCommand(() => SetResult(MessageBoxResult.Yes));
		NoCommand = new RelayCommand(() => SetResult(MessageBoxResult.No));
		CloseCommand = new RelayCommand(() => SetResult(MessageBoxResult.None));

		MinimizeCommand = new RelayCommand(MinimizeWindow);
	}

	private void MinimizeWindow()
	{
		var window = this.FindAncestorOfType<Window>();
		if (window != null)
		{
			window.WindowState = WindowState.Minimized;
		}
	}

	private void UpdateActualContent()
	{
		ActualContent = Content ?? Message;
	}

	private void OnHeaderBorderPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		HeaderPointerPressed?.Invoke(this, e);
	}
}
