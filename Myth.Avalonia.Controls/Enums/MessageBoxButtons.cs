namespace Myth.Avalonia.Controls.Enums
{
	[Flags]
	public enum MessageBoxButtons
	{
		None = 0,
		Ok = 1 << 0,      // 1
		Cancel = 1 << 1,  // 2
		Yes = 1 << 2,     // 4
		No = 1 << 3,      // 8

		// Combinations
		OkCancel = Ok | Cancel,           // 3
		YesNo = Yes | No,                 // 12
		YesNoCancel = Yes | No | Cancel   // 14
	}
}
