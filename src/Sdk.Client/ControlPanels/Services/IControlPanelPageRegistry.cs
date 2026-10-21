using Sdk.Client.ControlPanels.Attributes;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.Services;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Registry for control panel pages.
/// </summary>
[Obsolete("Use " + nameof(ControlPanelChildAttribute<>) + " and " + nameof(ControlPanelParentAttribute<>) +
    " to register a control panel as a child of another control panel instead. This type will be removed in a future major version.")]
public interface IControlPanelPageRegistry : IRegistry<IControlPanelPageRegistryItem>
{
    /// <summary>
    /// Registers a control panel page in the registry.
    /// </summary>
    /// <param name="controlPanelPage">The control panel page that should be registered in the registry.</param>
    /// <param name="controlPanelRegistryItem">The registry item of the control panel the page belongs to.</param>
    void Add(IControlPanelPage controlPanelPage, IControlPanelRegistryItem controlPanelRegistryItem);

    /// <summary>
    /// Removes a control panel page from the registry.
    /// </summary>
    /// <returns><see langword="true"/> when item was removed, otherwise <see langword="false"/>.</returns>
    bool Remove(IControlPanelPage controlPanelPage);
}
