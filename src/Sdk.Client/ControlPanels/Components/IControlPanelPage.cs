using Sdk.Client.ControlPanels.Attributes;

namespace Sdk.Client.ControlPanels.Components;

/// <summary>
/// Represents a single, navigable page within a control panel.
/// </summary>
[Obsolete("Use " + nameof(ControlPanelChildAttribute<>) + " and " + nameof(ControlPanelParentAttribute<>) +
    " to register a control panel as a child of another control panel instead. This type will be removed in a future major version.")]
public interface IControlPanelPage : IComponent
{
    /// <summary>
    /// Gets the title of the page, which is typically displayed in a tab or navigation element.
    /// </summary>
    string Title { get; }
}
