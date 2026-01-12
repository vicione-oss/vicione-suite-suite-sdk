namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Defines a provider for retrieving the currently active control panel descriptor.
/// </summary>
public interface IActiveControlPanelDescriptorProvider
{
    /// <summary>
    /// Gets the descriptor of the currently active control panel.
    /// </summary>
    IControlPanelDescriptor? GetActiveControlPanelDescriptor();
}
