using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A component that provides a consistent layout for a single field within a settings UI.
/// </summary>
public sealed partial class SettingsField : ComponentBase
{
    /// <summary>
    /// Optional text displayed above <see cref="Label"/> and <see cref="ChildContent"/>.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Optional text displayed on the left of <see cref="ChildContent"/>.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// <see langword="true"/> when a <see cref="LoadingIndication">loading indication</see> should be rendered,
    /// otherwise <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// <see cref="SettingsFieldLoadingIndication">Loading indication</see> that should be rendered when
    /// <see cref="IsLoading"/> is <see langword="true"/>, otherwise parameter is ignored.
    /// </summary>
    [Parameter]
    public SettingsFieldLoadingIndication LoadingIndication { get; set; }

    /// <summary>
    /// Renders the content of the settings field.
    /// </summary>
    /// <remarks>
    /// It is recommend to use provided input components like <see cref="SettingsFieldButton"/> or
    /// <see cref="SettingsFieldTextBox"/> for consistent styling and behavior accross the settings UI.
    /// </remarks>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
