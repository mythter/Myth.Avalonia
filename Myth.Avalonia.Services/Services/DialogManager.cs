using Avalonia;
using Avalonia.Controls;
using Myth.Avalonia.Services.Abstractions;

namespace Myth.Avalonia.Services;

public static class DialogManager
{
	// this dictionary stores the mapping
	private static readonly Dictionary<IDialogContext, Visual> _registrationMapper = [];

	static DialogManager()
	{
		// add a listener to changes of the attached register property
		RegisterProperty.Changed.AddClassHandler<Visual>(RegisterChanged);
	}

	private static void RegisterChanged(Visual sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (sender is null)
		{
			throw new InvalidOperationException("The DialogManager can only be registered on a Visual");
		}

		// Unregister any old registered context
		if (e.GetOldValue<IDialogContext>() is { } oldValue)
		{
			_registrationMapper.Remove(oldValue);
		}

		// Register any new context
		if (e.GetNewValue<IDialogContext>() is { } newValue)
		{
			_registrationMapper.Add(newValue, sender);
		}
	}

	/// <summary>
	/// This property handles the registration of Views and ViewModel
	/// </summary>
	public static readonly AttachedProperty<IDialogContext> RegisterProperty =
		AvaloniaProperty.RegisterAttached<Visual, IDialogContext>("Register", typeof(DialogManager));

	/// <summary>
	/// Accessor for attached property <see cref="RegisterProperty"/>.
	/// </summary>
	public static void SetRegister(AvaloniaObject element, IDialogContext value)
	{
		element.SetValue(RegisterProperty, value);
	}

	/// <summary>
	/// Accessor for attached property <see cref="RegisterProperty"/>.
	/// </summary>
	public static IDialogContext GetRegister(AvaloniaObject element)
	{
		return element.GetValue(RegisterProperty);
	}

	/// <summary>
	/// Gets the associated <see cref="Visual"/> for a given context. Returns null, if none was registered
	/// </summary>
	/// <param name="context">The context to lookup</param>
	/// <returns>The registered Visual for the context or null if none was found</returns>
	public static Visual? GetVisualForContext(IDialogContext context)
	{
		return _registrationMapper.GetValueOrDefault(context);
	}

	/// <summary>
	/// Gets the parent <see cref="TopLevel"/> for the given context. Returns null, if no TopLevel was found
	/// </summary>
	/// <param name="context">The context to lookup</param>
	/// <returns>The registered TopLevel for the context or null if none was found</returns>
	public static TopLevel? GetTopLevelForContext(IDialogContext context)
	{
		return TopLevel.GetTopLevel(GetVisualForContext(context));
	}
}
