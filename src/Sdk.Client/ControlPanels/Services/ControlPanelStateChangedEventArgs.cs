namespace Sdk.Client.ControlPanels.Services;

public sealed class ControlPanelStateChangedEventArgs(IControlPanelState sender, IEnumerable<string> propertyNames) : EventArgs
{
    public IControlPanelState Sender => sender;
    public IEnumerable<string> PropertyNames => propertyNames;
}
