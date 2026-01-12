using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// Provides the main layout structure for a settings page.
/// </summary>
public sealed partial class SettingsLayout : ComponentBase
{
    /// <summary>
    /// <see cref="RenderFragment"/> to place one or multiple <see cref="SettingsGroup"/> components
    /// to visually group individual settings.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
