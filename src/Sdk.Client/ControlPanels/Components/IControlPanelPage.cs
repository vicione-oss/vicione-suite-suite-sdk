namespace Sdk.Client.ControlPanels.Components;

/// <summary>
/// Represents a single, navigable page within a control panel.
/// </summary>
public interface IControlPanelPage : IComponent
{
    /// <summary>
    /// Gets the title of the page, which is typically displayed in a tab or navigation element.
    /// </summary>
    string Title { get; }
}
