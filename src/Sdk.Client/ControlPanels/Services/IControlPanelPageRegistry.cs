using Sdk.Client.ControlPanels.Components;
using Sdk.Client.Services;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Registry for control panel pages
/// </summary>
public interface IControlPanelPageRegistry : IRegistry<IControlPanelPageRegistryItem>
{
    /// <summary>
    /// Registers a control panel page in the registry
    /// </summary>
    /// <param name="controlPanelPage">Control panel page that should be registered in the registry</param>
    /// <param name="controlPanelRegistryItem">Registration associated with the control panel in which the control panel page lives</param>
    void Add(IControlPanelPage controlPanelPage, IControlPanelRegistryItem controlPanelRegistryItem);

    /// <summary>
    /// Removes a control panel page in the registry
    /// </summary>
    /// <returns>True when item was removed</returns>
    bool Remove(IControlPanelPage controlPanelPage);
}
