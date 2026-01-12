using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Represents a registered page within a <see cref="IControlPanelPageRegistry"/>.
/// </summary>
public interface IControlPanelPageRegistryItem
{
    /// <summary>
    /// Gets the registration item for the parent control panel that this page belongs to.
    /// </summary>
    IControlPanelRegistryItem ControlPanelRegistryItem { get; }

    /// <summary>
    /// Gets the instance of the control panel page component.
    /// </summary>
    IControlPanelPage ControlPanelPage { get; }
}
