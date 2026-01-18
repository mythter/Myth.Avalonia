using Avalonia;
using Avalonia.Controls;

using Myth.Avalonia.Controls.Enums;

namespace Myth.Avalonia.Controls.AttachedProperties
{
	public static class AutoCompleteBoxProperties
	{
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
