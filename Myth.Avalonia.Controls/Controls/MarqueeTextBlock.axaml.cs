using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using Color = Avalonia.Media.Color;

namespace Myth.Avalonia.Controls;

[TemplatePart("PART_Text", typeof(TextBlock))]
[TemplatePart("PART_Container", typeof(MarqueePanel))]
[TemplatePart("PART_LeftFade", typeof(Border))]
[TemplatePart("PART_RightFade", typeof(Border))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "It's okay")]
public class MarqueeTextBlock : TemplatedControl
{
	#region Private Fields

	private TextBlock? _textBlock;

	private MarqueePanel? _container;

	private Border? _leftFade;

	private Border? _rightFade;

	private Color _fadeColor;

	private double _offset = 0;

	private int _direction = -1; // -1 = left, 1 = right

	private CancellationTokenSource? _cts;

	#endregion

	#region Styled Properties

	public static readonly StyledProperty<string> TextProperty =
	AvaloniaProperty.Register<MarqueeTextBlock, string>(nameof(Text));

	public string Text
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public static readonly StyledProperty<double> SpeedProperty =
		AvaloniaProperty.Register<MarqueeTextBlock, double>(nameof(Speed), 1.5);

	public double Speed
	{
		get => GetValue(SpeedProperty);
		set => SetValue(SpeedProperty, value);
	}

	public static readonly StyledProperty<int> DelayProperty =
		AvaloniaProperty.Register<MarqueeTextBlock, int>(nameof(Delay), 800);

	public int Delay
	{
		get => GetValue(DelayProperty);
		set => SetValue(DelayProperty, value);
	}

	public static readonly StyledProperty<int> FadeSizeProperty =
		AvaloniaProperty.Register<MarqueeTextBlock, int>(nameof(FadeSize), 0);

	public int FadeSize
	{
		get => GetValue(FadeSizeProperty);
		set => SetValue(FadeSizeProperty, value);
	}

	#endregion

	#region Overrides

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_cts?.Cancel();

		_textBlock = e.NameScope.Get<TextBlock>("PART_Text");
		_container = e.NameScope.Get<MarqueePanel>("PART_Container");
		_leftFade = e.NameScope.Find<Border>("PART_LeftFade");
		_rightFade = e.NameScope.Find<Border>("PART_RightFade");

		_leftFade?.IsVisible = false;
		_rightFade?.IsVisible = false;

		ApplyFadeGradients();
		StartMarquee();
	}

	protected override void OnUnloaded(RoutedEventArgs e)
	{
		base.OnUnloaded(e);
		_cts?.Cancel();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == FadeSizeProperty)
			ApplyFadeGradients();

		if (change.Property == BackgroundProperty)
			UpdateFadeColor();
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		UpdateFadeColor();
	}

	#endregion

	#region Private Methods

	private void UpdateFadeColor()
	{
		Visual? current = this;
		while (current != null)
		{
			if (current is Control control)
			{
				if (control.GetValue(BackgroundProperty) is SolidColorBrush brush)
				{
					_fadeColor = brush.Color;
					ApplyFadeGradients();
					break;
				}
				else if (control.GetValue(BackgroundProperty) is ImmutableSolidColorBrush ibrush)
				{
					_fadeColor = ibrush.Color;
					ApplyFadeGradients();
					break;
				}
			}
			current = current.Parent as Visual;
		}

		ApplyFadeGradients();
	}

	private void ApplyFadeGradients()
	{
		var color = _fadeColor == Colors.Transparent ? Colors.Gray : _fadeColor;
		var transparent = Color.FromArgb(0, color.R, color.G, color.B);

		_leftFade?.Background = new LinearGradientBrush
		{
			StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
			EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
			GradientStops =
				[
					new GradientStop(color,       0.0),
					new GradientStop(transparent, 1.0),
				]
		};

		_rightFade?.Background = new LinearGradientBrush
		{
			StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
			EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
			GradientStops =
				[
					new GradientStop(transparent, 0.0),
					new GradientStop(color,       1.0),
				]
		};
	}

	private void StartMarquee()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		// retrieving values from UI thread
		var delay = Delay;
		var speed = Speed;

		Task.Run(async () =>
		{
			await Task.Delay(delay, token);

			while (!token.IsCancellationRequested)
			{
				await Task.Delay(16, token);

				bool hitEdge = false;

				await Dispatcher.UIThread.InvokeAsync(() =>
				{
					if (_textBlock == null || _container == null) return;

					double textWidth = _textBlock.DesiredSize.Width;
					double containerWidth = _container.Bounds.Width;

					if (textWidth <= containerWidth)
					{
						_offset = 0;
						_container.Offset = 0;
						_container.InvalidateArrange();
						_leftFade?.IsVisible = false;
						_rightFade?.IsVisible = false;
						return;
					}

					_leftFade?.IsVisible = true;
					_rightFade?.IsVisible = true;

					double maxOffset = textWidth - containerWidth;
					_offset += _direction * speed;

					if (_offset <= -maxOffset)
					{
						_offset = -maxOffset;
						_direction = 1;
						hitEdge = true;  // hitted right edge
					}
					else if (_offset >= 0)
					{
						_offset = 0;
						_direction = -1;
						hitEdge = true;  // hitted left edge
					}

					_container.Offset = _offset;
					_container.InvalidateArrange();
				});

				if (hitEdge)
					await Task.Delay(delay, token);
			}
		}, token);
	}

	#endregion
}
