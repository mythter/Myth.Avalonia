using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

using Myth.Avalonia.Controls.AttachedProperties;
using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.Behaviors
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "I don't think so")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<Pending>")]
	public class AutoCompleteDropdownButtonBehavior : Behavior<AutoCompleteBox>
	{
		private TextBox? _textBox;

		private Button? _dropDownButton;

		private Popup? _popup;

		private bool _popupPressed;

		static AutoCompleteDropdownButtonBehavior()
		{
		}

		protected override void OnAttached()
		{
			if (AssociatedObject is not null)
			{
				AssociatedObject.DropDownOpening += DropDownOpening;

				AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
			}

			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			RemoveDropdownButton();

			if (AssociatedObject is not null)
			{
				AssociatedObject.DropDownOpening -= DropDownOpening;
				AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
			}

			if (AssociatedObject?.GetVisualRoot() is Window window)
			{
				window.Deactivated -= OnWindowDeactivated;
			}

			_popup?.IsLightDismissEnabled = true; // giving back the original behavior

			_textBox?.GotFocus -= OnTextBoxGotFocus;

			_dropDownButton?.Click -= OnDropDownButtonClick;

			base.OnDetaching();
		}

		private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
			{
				_textBox = AssociatedObject?
					.GetType()
					.GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic)?
					.GetValue(AssociatedObject) as TextBox;

				_textBox?.GotFocus += OnTextBoxGotFocus;

				_popup = AssociatedObject?.GetVisualDescendants().OfType<Popup>().FirstOrDefault();

				// disable light dismiss so that clicking outside the popup doesn't close it
				_popup?.IsLightDismissEnabled = false;

				if (AssociatedObject?.GetVisualRoot() is Window window)
				{
					window.Deactivated += OnWindowDeactivated;
				}

				if (AutoCompleteBoxProperties.GetShowDropdownButton(AssociatedObject!))
				{
					CreateDropdownButton();
				}
			});
		}

		private void OnWindowDeactivated(object? sender, EventArgs e)
		{
			_popup?.IsOpen = false;
		}

		private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
		{
			if (_popupPressed)
			{
				_popupPressed = false;
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
			}
		}
		private void CreateDropdownButton()
		{
			if (AssociatedObject is null || _textBox is null)
				return;

			var padding = _textBox.Padding.Top;
			var fontFamily = (FontFamily)Application.Current?.Resources["Phosphor"]!;

			var cornerRadius = _textBox.CornerRadius;
			var border = _textBox.BorderThickness;

			var side = AutoCompleteBoxProperties.GetDropdownButtonPosition(AssociatedObject);

			CornerRadius buttonRadius;
			int defaultRadius = 3;

			if (side == AutoCompleteBoxDropdownButtonPosition.Left)
			{
				buttonRadius = new CornerRadius(
					Math.Max(0, cornerRadius.TopLeft - border.Left),
					defaultRadius,
					defaultRadius,
					Math.Max(defaultRadius, cornerRadius.BottomLeft - border.Left));
			}
			else // Right side
			{
				buttonRadius = new CornerRadius(
					defaultRadius,
					Math.Max(defaultRadius, cornerRadius.TopRight - border.Right),
					Math.Max(defaultRadius, cornerRadius.BottomRight - border.Right),
					defaultRadius);
			}

			_dropDownButton = new Button()
			{
				Content = new TextBlock()
				{
					Text = "\uEB04",
					FontSize = _textBox.FontSize,
					FontFamily = fontFamily,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				},
				Padding = new Thickness(1, padding, 1, padding),
				Focusable = false,
				ClickMode = ClickMode.Press,
				VerticalAlignment = VerticalAlignment.Stretch,
				CornerRadius = buttonRadius
			};

			_dropDownButton.Click += OnDropDownButtonClick;

			if (side == AutoCompleteBoxDropdownButtonPosition.Left)
				_textBox.InnerLeftContent = _dropDownButton;
			else
				_textBox.InnerRightContent = _dropDownButton;
		}

		private void OnDropDownButtonClick(object? sender, RoutedEventArgs e)
		{
			if (AssociatedObject?.IsDropDownOpen ?? false)
			{
				AssociatedObject.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, false);
			}
			else
			{
				ShowDropdown();
			}
		}

		private void OnPopupPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			_popupPressed = true;
		}

		private void RemoveDropdownButton()
		{
			_textBox?.InnerLeftContent = null;
		}
	}
}
