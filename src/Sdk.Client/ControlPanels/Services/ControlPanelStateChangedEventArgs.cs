namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Provides data for the event that is raised when a property of a control panel's state has changed.
/// </summary>
public sealed class ControlPanelStateChangedEventArgs(IControlPanelState sender, IEnumerable<string> propertyNames) : EventArgs
{
    /// <summary>
    /// Gets the state object that raised the event.
    /// </summary>
    public IControlPanelState Sender => sender;

    /// <summary>
    /// Gets a collection of the names of the properties that have changed.
    /// </summary>
    public IEnumerable<string> PropertyNames => propertyNames;
}
