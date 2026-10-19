using Sdk.Client.ControlPanels.Attributes;
using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Represents a registered page within a <see cref="IControlPanelPageRegistry"/>.
/// </summary>
[Obsolete("Use " + nameof(ControlPanelChildAttribute<>) + " and " + nameof(ControlPanelParentAttribute<>) +
    " to register a control panel as a child of another control panel instead. This type will be removed in a future major version.")]
#pragma warning disable CS0618 // Type or member is obsolete
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
#pragma warning restore CS0618 // Type or member is obsolete
