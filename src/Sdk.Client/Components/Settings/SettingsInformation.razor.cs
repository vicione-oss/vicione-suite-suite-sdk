using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A component that renders a block of informational content within a settings UI.
/// </summary>
public sealed partial class SettingsInformation : ComponentBase
{
    /// <summary>
    /// Gets or sets the content to be displayed within the information block.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
