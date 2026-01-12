using Microsoft.AspNetCore.Components;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// Renders a description with title, icon and content when placed inside a
/// <see cref="IControlPanel"/> or <see cref="ControlPanelPage"/>.
/// </summary>
/// <remarks>
/// If neither <see cref="IconCssClass"/> nor <see cref="IconUrl"/> is specified
/// then <see cref="IControlPanelDescriptor.IconPath"/> is used as a fallback.
/// </remarks>
public sealed partial class DescriptionBanner : ComponentBase
{
    [CascadingParameter]
    private DescriptionBannerSectionId SectionId { get; set; } = default!;

    /// <summary>
    /// CSS class that defines the icon displayed next to the <see cref="Title"/>
    /// </summary>
    /// <remarks>
    /// This property has priority over <see cref="IconUrl"/>.
    /// </remarks>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Url of the icon displayed next to the <see cref="Title"/>
    /// </summary>
    /// <remarks>
    /// This property has lower priority than <see cref="IconCssClass"/>.
    /// </remarks>
    [Parameter]
    public Uri? IconUrl { get; set; }

    /// <summary>
    /// Title displayed next to the icon
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; }

    /// <summary>
    /// Description content
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }

    [Inject]
    private IActiveControlPanelDescriptorProvider ActiveControlPanelDescriptorProvider { get; set; } = default!;
}
