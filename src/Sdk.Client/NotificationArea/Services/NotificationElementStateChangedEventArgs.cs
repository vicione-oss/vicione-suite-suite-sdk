namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Arguments for event <see cref="INotificationElementState.Changed"/>
/// </summary>
public sealed class NotificationElementStateChangedEventArgs(INotificationElementState sender, IEnumerable<string> propertyNames)
    : EventArgs
{
    /// <summary>
    /// Gets the state object that raised the event.
    /// </summary>
    public INotificationElementState Sender => sender;

    /// <summary>
    /// Gets a collection of the names of the properties that have changed.
    /// </summary>
    public IEnumerable<string> PropertyNames => propertyNames;
}
