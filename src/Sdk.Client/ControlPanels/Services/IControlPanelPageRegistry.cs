using Sdk.Client.ControlPanels.Components;
using Sdk.Client.Services;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Registry for control panel pages.
/// </summary>
public interface IControlPanelPageRegistry : IRegistry<IControlPanelPageRegistryItem>
{
    /// <summary>
    /// Registers a control panel page in the registry.
    /// </summary>
    /// <param name="controlPanelPage">The control panel page that should be registered in the registry.</param>
    /// <param name="controlPanelRegistryItem">The registration associated with the control panel in which the control panel page lives.</param>
    void Add(IControlPanelPage controlPanelPage, IControlPanelRegistryItem controlPanelRegistryItem);

    /// <summary>
    /// Removes a control panel page in the registry.
    /// </summary>
    /// <returns><see langword="true"/> when item was removed, otherwise <see langword="false"/>.</returns>
    bool Remove(IControlPanelPage controlPanelPage);
}
