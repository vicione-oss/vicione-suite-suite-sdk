namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Hook to execute update logic for control panel registries before control panel registries are enumerated.
/// </summary>
public interface IUpdateControlPanelRegistryHandler
{
    /// <summary>
    /// Executes update logic for control panel registries.
    /// </summary>
    Task Execute(CancellationToken cancellationToken);
}
