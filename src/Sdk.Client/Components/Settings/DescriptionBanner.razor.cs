using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// Renders a description with title, icon and content when placed inside a
/// <see cref="IControlPanel"/> or <see cref="ControlPanelPage"/>.
/// </summary>
/// <remarks>
/// If neither <see cref="IconCssClass"/> nor <see cref="IconUrl"/> is specified
/// then <see cref="IControlPanelDescriptor.IconUrl"/> is used as a fallback.
/// </remarks>
public sealed partial class DescriptionBanner : ComponentBase
{
    [CascadingParameter]
    private DescriptionBannerSectionId SectionId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the CSS class of the icon next to the <see cref="Title"/>; takes priority over <see cref="IconUrl"/>.
    /// </summary>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets the URL of the icon next to the <see cref="Title"/>; used only when <see cref="IconCssClass"/> is not set.
    /// </summary>
    [Parameter]
    public Uri? IconUrl { get; set; }

    /// <summary>
    /// Gets or sets the title displayed next to the icon.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the description text.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }

    [Inject]
    private IActiveControlPanelDescriptorProvider ActiveControlPanelDescriptorProvider { get; set; } = default!;
}
