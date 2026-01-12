using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Provides the active control panel page for a control panel
/// </summary>
public interface IActiveControlPanelPageProvider
{
    /// <returns>
    /// Active control panel page for the control panel represented by
    /// the given <paramref name="controlPanelRegistryItem"/>
    /// </returns>
    IControlPanelPage? GetActiveControlPanelPage(IControlPanelRegistryItem controlPanelRegistryItem);
}
