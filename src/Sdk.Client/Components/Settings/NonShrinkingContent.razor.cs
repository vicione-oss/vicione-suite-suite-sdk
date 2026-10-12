namespace Sdk.Client.Components.Settings;

/// <summary>
/// Keeps its content at full height inside a <see cref="LimitedHeightLayout"/>, so only the other children give up room.
/// </summary>
public sealed partial class NonShrinkingContent : ComponentBase
{
    /// <summary>
    /// Gets or sets the content that keeps its natural height.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
