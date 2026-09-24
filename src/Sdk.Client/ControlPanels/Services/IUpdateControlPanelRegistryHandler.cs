namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Updates the control panel registries before they are enumerated.
/// </summary>
public interface IUpdateControlPanelRegistryHandler
{
    /// <summary>
    /// Updates the registries, e.g. adds or removes control panels that depend on runtime state.
    /// </summary>
    Task Execute(CancellationToken cancellationToken);
}
