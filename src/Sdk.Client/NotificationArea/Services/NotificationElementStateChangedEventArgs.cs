namespace Sdk.Client.NotificationArea.Services;

public sealed class NotificationElementStateChangedEventArgs(INotificationElementState sender, IEnumerable<string> propertyNames) : EventArgs
{
    public INotificationElementState Sender => sender;
    public IEnumerable<string> PropertyNames => propertyNames;
}
