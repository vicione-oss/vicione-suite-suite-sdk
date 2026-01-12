using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// Renders a block of informational content
/// </summary>
public sealed partial class SettingsInformation : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
