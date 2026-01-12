using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.NotificationBar;

public sealed partial class ContentCardComponent : ComponentBase
{
    [Parameter]
    public string? Heading { get; set; }

    [Parameter]
    public RenderFragment? LeftSideContent { get; set; }

    [Parameter]
    public RenderFragment? MainContent { get; set; }
}
