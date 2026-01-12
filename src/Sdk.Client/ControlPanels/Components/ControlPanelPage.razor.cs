using Microsoft.AspNetCore.Components;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Components;

/// <summary>
/// Represents a single, navigable page within a control panel.
/// </summary>
public sealed partial class ControlPanelPage : ComponentBase, IControlPanelPage, IDisposable
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered on this page.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }

    [CascadingParameter]
    private IControlPanelRegistryItem ControlPanelRegistryItem { get; set; } = default!;

    [Inject]
    private IControlPanelPageRegistry ControlPanelPageRegistry { get; set; } = default!;

    [Inject]
    private IActiveControlPanelPageProvider ActiveControlPanelPageProvider { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
        => ControlPanelPageRegistry.Add(this, ControlPanelRegistryItem);

    /// <inheritdoc/>
    public void Dispose()
        => ControlPanelPageRegistry.Remove(this);
}
