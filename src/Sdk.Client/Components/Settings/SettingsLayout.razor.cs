namespace Sdk.Client.Components.Settings;

/// <summary>
/// Provides the main layout structure for a settings page.
/// </summary>
public sealed partial class SettingsLayout : ComponentBase
{
    /// <summary>
    /// Gets or sets the page's content: one or more <see cref="SettingsGroup"/> components.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
