using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

public sealed partial class SettingsLayout : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
