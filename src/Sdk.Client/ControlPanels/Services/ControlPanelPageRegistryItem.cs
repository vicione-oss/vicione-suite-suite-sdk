using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

internal sealed record ControlPanelPageRegistryItem(
    IControlPanelPage ControlPanelPage, IControlPanelRegistryItem ControlPanelRegistryItem) : IControlPanelPageRegistryItem;
