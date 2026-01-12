using Sdk.Client.Extensions;

namespace Sdk.Client.Colors;

public sealed class StandardHtmlColor : IHtmlColor
{
    private readonly StandardColor _standardColor;
    private readonly string _cssCustomVariable;

    private StandardHtmlColor(StandardColor standardColor)
    {
        _standardColor = standardColor;
        _cssCustomVariable = $"var(--vo-standard-color-{_standardColor.ToString().ToHyphenSeparated()})";
    }

    public static StandardHtmlColor From(StandardColor standardColor) => new(standardColor);

    public override string ToString() => _cssCustomVariable;
}
