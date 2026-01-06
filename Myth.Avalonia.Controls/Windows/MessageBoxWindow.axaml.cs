using Avalonia.Controls;
using Avalonia.Input;

using Myth.Avalonia.Controls.Enums;
using Myth.Avalonia.Controls.Options;

namespace Myth.Avalonia.Controls.Windows;

public partial class MessageBoxWindow : Window
{
	public MessageBoxWindow()
	{
		InitializeComponent();
	}

	public MessageBoxWindow(MessageBoxOptions options)
	{
		InitializeComponent();

		MessageBox.Options = options;
		MessageBox.Owner = this;
	}

	private void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			BeginMoveDrag(e);
		}
	}

	private void OnClose(object? sender, MessageBoxResult result)
	{
		Close(result);
	}
}