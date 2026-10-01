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

    /// <summary>
    /// Gets or sets the space in pixels kept free right of field contents, information texts, steppers and nested group expanders;
    /// <see langword="null"/> keeps the default of 138 px, sized for settings dialogs and control panels.
    /// </summary>
    [Parameter]
    public int? ContentPaddingRight { get; set; }

    private string? Style => ContentPaddingRight is { } contentPaddingRight
        ? $"--settings-content-padding-right: {contentPaddingRight}px;"
        : null;
}
