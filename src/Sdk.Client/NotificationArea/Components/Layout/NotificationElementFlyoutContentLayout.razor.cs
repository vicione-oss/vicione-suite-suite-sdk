namespace Sdk.Client.NotificationArea.Components.Layout;

/// <summary>
/// Provides a standard layout structure for the content of a notification element's flyout.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed partial class NotificationElementFlyoutContentLayout : ComponentBase
{
    /// <summary>
    /// Gets or sets the content to be displayed in the main area of the flyout.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment Main { get; set; }

    /// <summary>
    /// Gets or sets the content to be displayed in the footer area of the flyout.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment Footer { get; set; }
}
