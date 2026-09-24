namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Describes a category that groups control panels.
/// </summary>
public interface IControlPanelCategoryDescriptor
{
    /// <summary>
    /// Gets the category's title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the CSS class of the icon next to <see cref="Title"/>; takes priority over <see cref="IconUrl"/>.
    /// Defaults to <see langword="null"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    string? IconCssClass => null;

    /// <summary>
    /// Gets the URL of the icon next to <see cref="Title"/>; used only when <see cref="IconCssClass"/> is <see langword="null"/>.
    /// Defaults to <see langword="null"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    Uri? IconUrl => null;

    /// <summary>
    /// Gets the category's position within its <see cref="IControlPanelGroupDescriptor">group</see>; lower positions render first.
    /// Categories without a position follow, ordered by <see cref="Title"/>. Defaults to <see langword="null"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    int? Position => null;
}
