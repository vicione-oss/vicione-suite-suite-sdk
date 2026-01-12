using Microsoft.AspNetCore.Components;

namespace Sdk.Client.ControlPanels.Components;

/// <summary>
/// Describes a control panel page
/// </summary>
public interface IControlPanelPage : IComponent
{
    string Title { get; }
}
