namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Arguments for event <see cref="IControlPanelRequest.ControlPanelRequested"/>
/// </summary>
/// <param name="registryItem">Registry item associated with the requested control panel</param>
/// <param name="configureState">Configures the state associated with the requested control panel</param>
public sealed class ControlPanelRequestedEventArgs(IControlPanelRegistryItem registryItem,
    Action? configureState) : EventArgs
{
    /// <summary>
    /// Registry item associated with the requested control panel.
    /// </summary>
    public IControlPanelRegistryItem RegistryItem => registryItem;

    /// <summary>
    /// Configures the state associated with the requested control panel.
    /// </summary>
    public Action? ConfigureState => configureState;

    /// <summary>
    /// True if the request should be canceled, otherwise false.
    /// </summary>
    public bool Cancel { get; set; }
}
