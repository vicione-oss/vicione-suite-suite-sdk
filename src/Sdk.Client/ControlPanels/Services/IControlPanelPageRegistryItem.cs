using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Item describing a control panel page registration
/// </summary>
public interface IControlPanelPageRegistryItem
{
    IControlPanelRegistryItem ControlPanelRegistryItem { get; }
    IControlPanelPage ControlPanelPage { get; }
}
