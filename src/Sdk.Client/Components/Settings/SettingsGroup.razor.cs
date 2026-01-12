using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

public sealed partial class SettingsGroup : ComponentBase
{
    [Parameter, EditorRequired]
    public string Title { get; set; }

    /// <summary>
    /// Text displayed below <see cref="Title"/>
    /// </summary>
    [Parameter]
    public required string? Subline { get; set; }

    /// <summary>
    /// True when expanded, false when not expanded, null when always expanded and no expander should be displayed
    /// </summary>
    [Parameter]
    public bool? Expanded { get; set; }

    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// RenderFragment to render a custom expander
    /// </summary>
    /// <remarks>
    /// If this property is not set then a default expander is rendered, but only when <see cref="Expanded"/> is not null.
    /// </remarks>
    [Parameter]
    public RenderFragment? Expander { get; set; }

    /// <summary>
    /// RenderFragment to render an optional element before the expander
    /// </summary>
    [Parameter]
    public RenderFragment? BeforeExpander { get; set; }

    /// <summary>
    /// RenderFragment to place <see cref="SettingsField"/>, <see cref="SettingsInformation"/> or
    /// <see cref="SettingsGroup"/> components
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    internal SettingsGroup? ParentSettingsGroup { get; set; }

    private async Task ExpanderButtonClick(bool expandedNew)
    {
        if (ExpandedChanged.HasDelegate)
            await ExpandedChanged.InvokeAsync(expandedNew);
    }
}
