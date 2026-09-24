using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Tells which page of a control panel is currently shown.
/// </summary>
public interface IActiveControlPanelPageProvider
{
    /// <summary>
    /// Returns the page shown for the control panel of <paramref name="controlPanelRegistryItem"/>; <see langword="null"/> if none is.
    /// </summary>
    IControlPanelPage? GetActiveControlPanelPage(IControlPanelRegistryItem controlPanelRegistryItem);
}
