using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.NotificationBar;

/// <summary>
/// A component that displays content in a card format, typically used within a notification bar.
/// </summary>
public sealed partial class ContentCardComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the optional heading text displayed at the top of the card.
    /// </summary>
    [Parameter]
    public string? Heading { get; set; }

    /// <summary>
    /// Gets or sets the content to be displayed on the left side of the card, typically for an icon or status indicator.
    /// </summary>
    [Parameter]
    public RenderFragment? LeftSideContent { get; set; }

    /// <summary>
    /// Gets or sets the main content to be displayed in the card.
    /// </summary>
    [Parameter]
    public RenderFragment? MainContent { get; set; }
}
