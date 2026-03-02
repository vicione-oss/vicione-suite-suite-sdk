namespace Sdk.Client.Components.Settings;

/// <summary>
/// A component that visually groups related settings fields.
/// </summary>
public sealed partial class SettingsGroup : ComponentBase
{
    /// <summary>
    /// Gets or sets the title displayed for the settings group.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the descriptive text displayed below the title.
    /// </summary>
    [Parameter]
    public required string? Subline { get; set; }

    /// <summary>
    /// Gets or sets the expanded state of the group.
    /// </summary>
    /// <value>
    /// <see langword="true"/> when expanded, <see langword="false"/> when not expanded,
    /// <see langword="null"/> when always expanded and no expander should be displayed.
    /// </value>
    [Parameter]
    public bool? Expanded { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the expanded state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="RenderFragment"/> for a custom expander control.
    /// </summary>
    /// <remarks>
    /// If not set, a default expander is rendered, provided that <see cref="Expanded"/> is not <see langword="null"/>.
    /// </remarks>
    [Parameter]
    public RenderFragment? Expander { get; set; }

    /// <summary>
    /// Gets or sets an optional <see cref="RenderFragment"/> to be rendered before the expander control.
    /// </summary>
    [Parameter]
    public RenderFragment? BeforeExpander { get; set; }

    /// <summary>
    /// Optional <see cref="RenderFragment"/> to place one or multiple <see cref="SettingsField"/>,
    /// <see cref="SettingsInformation"/> and / or <see cref="SettingsGroup"/> components.
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
