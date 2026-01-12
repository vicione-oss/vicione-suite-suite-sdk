using Microsoft.AspNetCore.Components;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Components;

public sealed partial class ControlPanelPage : ComponentBase, IControlPanelPage, IDisposable
{
    [Parameter, EditorRequired]
    public string Title { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }

    [CascadingParameter]
    private IControlPanelRegistryItem ControlPanelRegistryItem { get; set; } = default!;

    [Inject]
    private IControlPanelPageRegistry ControlPanelPageRegistry { get; set; } = default!;

    [Inject]
    private IActiveControlPanelPageProvider ActiveControlPanelPageProvider { get; set; } = default!;

    protected override void OnInitialized()
        => ControlPanelPageRegistry.Add(this, ControlPanelRegistryItem);

    public void Dispose()
        => ControlPanelPageRegistry.Remove(this);
}
