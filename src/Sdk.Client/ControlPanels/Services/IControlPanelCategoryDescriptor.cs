namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Describes a control panel category
/// </summary>
public interface IControlPanelCategoryDescriptor
{
    /// <summary>
    /// Text used to categorize the settings provided by the control panel
    /// </summary>
    string Title { get; }

    /// <summary>
    /// CSS class that defines the icon displayed next to the <see cref="Title"/>
    /// </summary>
    /// <remarks>
    /// This property has priority over <see cref="IconUrl"/>.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    string? IconCssClass => null;

    /// <summary>
    /// Url of the icon displayed next to the <see cref="Title"/>
    /// </summary>
    /// <remarks>
    /// This property has lower priority than <see cref="IconCssClass"/>.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    Uri? IconUrl => null;

    /// <summary>
    /// Optional position in the list of all category of a <see cref="IControlPanelGroupDescriptor">group</see>
    /// </summary>
    /// <remarks>
    /// This property affects the render order.
    /// When Position X of category A is lower than Position Y of category B then category A is rendered first.
    /// In a vertical representation this would mean that category A is displayed above category B.
    ///
    /// If not set then the category is rendered after all categories having a position in alphabetic order using <see cref="Title"/>.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    int? Position => null;
}
