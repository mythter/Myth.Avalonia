using Avalonia;
using Avalonia.Controls;

namespace Myth.Avalonia.Controls;

internal class MarqueePanel : Panel
{
	public double Offset { get; set; } = 0;

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (var child in Children)
			child.Measure(new Size(double.PositiveInfinity, availableSize.Height));

		return new Size(availableSize.Width, Children.FirstOrDefault()?.DesiredSize.Height ?? 0);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		foreach (var child in Children)
		{
			child.Arrange(new Rect(
				Offset, // X offset
				0,
				child.DesiredSize.Width, // full text width
				finalSize.Height
			));
		}

		return finalSize;
	}
}
