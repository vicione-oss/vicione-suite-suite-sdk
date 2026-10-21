using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Tells which page of a control panel is currently shown.
/// </summary>
[Obsolete("Nest control panels instead, via " + nameof(IControlPanelRegistryItem) + "." +
    nameof(IControlPanelRegistryItem.ChildOf) + ". This type will be removed in a future major version.")]
public interface IActiveControlPanelPageProvider
{
    /// <summary>
    /// Returns the page shown for the control panel of <paramref name="controlPanelRegistryItem"/>; <see langword="null"/> if none is.
    /// </summary>
    IControlPanelPage? GetActiveControlPanelPage(IControlPanelRegistryItem controlPanelRegistryItem);
}
