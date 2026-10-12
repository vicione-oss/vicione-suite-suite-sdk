namespace Sdk.Client.Components.Settings;

/// <summary>
/// Limits its content to the available height.
/// </summary>
/// <remarks>
/// Children shrink to fit, so consider using content that scrolls or scales by itself as children.
/// <para>
/// Wrap content that neither scrolls nor scales by itself in <see cref="NonShrinkingContent"/> to keep its height.
/// </para>
/// </remarks>
public sealed partial class LimitedHeightLayout : ComponentBase
{
    /// <summary>
    /// Gets or sets the content, stacked vertically 16 px apart like settings groups.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
