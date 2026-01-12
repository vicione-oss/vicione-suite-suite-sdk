using Microsoft.AspNetCore.Components;

namespace Sdk.Client.NotificationArea.Components.Layout;

public sealed partial class NotificationElementFlyoutContentLayout : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment Main { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment Footer { get; set; }
}
