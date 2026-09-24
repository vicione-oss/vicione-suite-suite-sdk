namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Arguments of <see cref="IControlPanelRequest.ControlPanelRequested"/>.
/// </summary>
/// <param name="registryItem">The registry item of the requested control panel.</param>
/// <param name="configureState">Configures the requested control panel's state; <see langword="null"/> leaves it unchanged.</param>
[ExcludeFromCodeCoverage]
public sealed class ControlPanelRequestedEventArgs(IControlPanelRegistryItem registryItem,
    Action? configureState) : EventArgs
{
    /// <summary>
    /// Gets the registry item of the requested control panel.
    /// </summary>
    public IControlPanelRegistryItem RegistryItem => registryItem;

    /// <summary>
    /// Gets the action that configures the requested control panel's state; <see langword="null"/> if none was given.
    /// </summary>
    public Action? ConfigureState => configureState;

    /// <summary>
    /// Gets or sets whether a handler cancels the request; <see cref="IControlPanelRequest.Send(IControlPanelRegistryItem, Action)"/>
    /// reports it to the caller.
    /// </summary>
    public bool Cancel { get; set; }
}
