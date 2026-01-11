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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "I don't think so")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<Pending>")]
	public class AutoCompleteZeroMinimumPrefixLengthDropdownBehavior : Behavior<AutoCompleteBox>
	{
		private TextBox? _textBox;

		private Popup? _popup;

		private bool _popupPressed;

		protected override void OnAttached()
		{
			if (AssociatedObject is not null)
			{
				AssociatedObject.KeyUp += OnKeyUp;
				AssociatedObject.DropDownOpening += DropDownOpening;

				AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
			}

			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			if (AssociatedObject is not null)
			{
				AssociatedObject.KeyUp -= OnKeyUp;
				AssociatedObject.DropDownOpening -= DropDownOpening;
				AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
			}

			_textBox?.GotFocus -= OnTextBoxGotFocus;

			_popup?.IsLightDismissEnabled = true; // giving back the original behavior

			_popup?.RemoveHandler(InputElement.PointerPressedEvent, OnPopupPointerPressed);

			base.OnDetaching();
		}

		private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
			{
				_popup = AssociatedObject?.GetVisualDescendants().OfType<Popup>().FirstOrDefault();

				_textBox = AssociatedObject?
					.GetType()
					.GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic)?
					.GetValue(AssociatedObject) as TextBox;

				_textBox?.GotFocus += OnTextBoxGotFocus;

				// disable light dismiss so that clicking outside the popup doesn't close it
				_popup?.IsLightDismissEnabled = false;

				_popup?.AddHandler(
					InputElement.PointerPressedEvent,
					OnPopupPointerPressed,
					RoutingStrategies.Tunnel,
					handledEventsToo: true
				);

				if (AssociatedObject?.GetVisualRoot() is Window window)
				{
					window.Deactivated += (_, _) =>
					{
						_popup?.IsOpen = false;
					};
				}
			});
		}

		//have to use KeyUp as AutoCompleteBox eats some of the KeyDown events
		private void OnKeyUp(object? sender, KeyEventArgs e)
		{
			if ((e.Key == Key.Down || e.Key == Key.F4) && string.IsNullOrEmpty(AssociatedObject?.Text))
			{
				ShowDropdown();
			}
		}

		private void DropDownOpening(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_textBox?.IsReadOnly ?? false)
			{
				e.Cancel = true;
			}
		}

		private void ShowDropdown()
		{
			if (AssociatedObject is not null && !AssociatedObject.IsDropDownOpen)
			{
				Dispatcher.UIThread.Post(() =>
				{
					typeof(AutoCompleteBox).GetMethod("PopulateDropDown", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(AssociatedObject, [AssociatedObject, EventArgs.Empty]);
					typeof(AutoCompleteBox).GetMethod("OpeningDropDown", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(AssociatedObject, [false]);

					if (!AssociatedObject.IsDropDownOpen)
					{
						//We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
						if (typeof(AutoCompleteBox)
								.GetField("_ignorePropertyChange", BindingFlags.NonPublic | BindingFlags.Instance) is { } ipc &&
							ipc.GetValue(AssociatedObject) is false)
						{
							ipc?.SetValue(AssociatedObject, true);
						}

						AssociatedObject.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
					}
				});
			}
		}

		private void OnPopupPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			_popupPressed = true;
		}

		private void OnTextBoxGotFocus(object? sender, RoutedEventArgs e)
		{
			if (_popupPressed)
			{
				_popupPressed = false;
				return;
			}

			ShowDropdown();
		}
	}
}
