namespace Sdk.Client.Components.Settings;

/// <summary>
/// A component that provides a consistent layout for a single field within a settings UI.
/// </summary>
public sealed partial class SettingsField : ComponentBase
{
    /// <summary>
    /// Gets or sets the optional text displayed above <see cref="Label"/> and <see cref="ChildContent"/>.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the optional text displayed to the left of <see cref="ChildContent"/>.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets whether the <see cref="LoadingIndication"/> is rendered.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets the loading indication rendered while <see cref="IsLoading"/> is <see langword="true"/>.
    /// </summary>
    [Parameter]
    public SettingsFieldLoadingIndication LoadingIndication { get; set; }

    /// <summary>
    /// Gets or sets the field's input. The <c>SettingsField*</c> components, such as <see cref="SettingsFieldButton"/> or
    /// <see cref="SettingsFieldTextBox"/>, keep styling and behavior consistent across the settings UI.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
