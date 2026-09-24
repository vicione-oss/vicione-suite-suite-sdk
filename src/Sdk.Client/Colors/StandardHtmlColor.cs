using Sdk.Client.Extensions;

namespace Sdk.Client.Colors;

/// <summary>
/// Represents a standard, predefined color as an HTML-compatible color string that references a CSS custom property.
/// </summary>
public sealed class StandardHtmlColor : IHtmlColor
{
    private readonly StandardColor _standardColor;
    private readonly string _cssCustomVariable;

    private StandardHtmlColor(StandardColor standardColor)
    {
        _standardColor = standardColor;
        _cssCustomVariable = $"var(--vo-standard-color-{_standardColor.ToString().ToHyphenSeparated()})";
    }

    /// <summary>
    /// Creates a new <see cref="StandardHtmlColor"/> instance from a <see cref="StandardColor"/> enum value.
    /// </summary>
    public static StandardHtmlColor From(StandardColor standardColor) => new(standardColor);

    /// <summary>
    /// Returns the CSS custom property reference, e.g. <c>var(--vo-standard-color-light-green)</c>.
    /// </summary>
    public override string ToString() => _cssCustomVariable;
}
