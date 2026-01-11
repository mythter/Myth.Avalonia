using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Myth.Avalonia.Controls.Behaviors
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<Pending>")]
	public class AutoCompleteResetCaretPositionBehavior : Behavior<AutoCompleteBox>
	{
		private TextBox? _textBox;

		private Popup? _popup;

		private bool _popupPressed;

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject!.AttachedToVisualTree += OnAttachedToVisualTree;
			AssociatedObject!.LostFocus += OnLostFocus;
		}

		protected override void OnDetaching()
		{
			AssociatedObject!.AttachedToVisualTree -= OnAttachedToVisualTree;
			AssociatedObject!.LostFocus -= OnLostFocus;

			_popup?.RemoveHandler(InputElement.PointerPressedEvent, OnPopupPointerPressed);

			base.OnDetaching();
		}

		private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
			{
				var prop = AssociatedObject?.GetType().GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic);

				if (prop?.GetValue(AssociatedObject) is not TextBox tb)
					return;

				_textBox = tb;

				_popup = AssociatedObject?.GetVisualDescendants().OfType<Popup>().FirstOrDefault();

				_popup?.AddHandler(
					InputElement.PointerPressedEvent,
					OnPopupPointerPressed,
					RoutingStrategies.Tunnel,
					handledEventsToo: true
				);
			});
		}

		private void OnPopupPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			_popupPressed = true;

			Dispatcher.UIThread.Post(() =>
			{
				_popupPressed = false;
			});
		}

		private void OnLostFocus(object? sender, RoutedEventArgs e)
		{
			if (!_popupPressed && _textBox != null)
			{
				_textBox.CaretIndex = 0;
			}
		}
	}
}
