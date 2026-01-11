using Avalonia;
using Avalonia.Controls;

using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.AttachedProperties
{
	public static class AutoCompleteBoxProperties
	{
		#region ShowDropdownButton

		public static readonly AttachedProperty<bool> ShowDropdownButtonProperty =
			AvaloniaProperty.RegisterAttached<Control, bool>(
				"ShowDropdownButton",
				typeof(AutoCompleteBoxProperties),
				defaultValue: false);

		public static bool GetShowDropdownButton(Control control) =>
			control.GetValue(ShowDropdownButtonProperty);

		public static void SetShowDropdownButton(Control control, bool value) =>
			control.SetValue(ShowDropdownButtonProperty, value);

		#endregion

		#region DropdownButtonPosition

		public static readonly AttachedProperty<AutoCompleteBoxDropdownButtonPosition> DropdownButtonPositionProperty =
		AvaloniaProperty.RegisterAttached<Control, AutoCompleteBoxDropdownButtonPosition>(
			"DropdownButtonPosition",
			typeof(AutoCompleteBoxProperties),
			AutoCompleteBoxDropdownButtonPosition.Left);

		public static AutoCompleteBoxDropdownButtonPosition GetDropdownButtonPosition(Control control) =>
			control.GetValue(DropdownButtonPositionProperty);

		public static void SetDropdownButtonPosition(Control control, AutoCompleteBoxDropdownButtonPosition value) =>
			control.SetValue(DropdownButtonPositionProperty, value);

		#endregion
	}
}
