using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

public sealed partial class SettingsField : ComponentBase
{
    /// <summary>
    /// Optional text displayed above <see cref="Label"/> and <see cref="ChildContent"/>
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Optional text displayed on the left of <see cref="ChildContent"/>
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// <see langword="true"/> when a <see cref="LoadingIndication">loading indication</see> should be rendered, otherwise <see langword="false"/>
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// <see cref="SettingsFieldLoadingIndication">Loading indication</see> that should be rendered when <see cref="IsLoading"/> is <see langword="true"/>, otherwise parameter is ignored
    /// </summary>
    [Parameter]
    public SettingsFieldLoadingIndication LoadingIndication { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
