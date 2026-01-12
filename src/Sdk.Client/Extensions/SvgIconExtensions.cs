using Sdk.Client.Enums;

namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="SvgIcon"/>.
/// </summary>
public static class SvgIconExtensions
{
    /// <summary>
    /// Constructs the relative URL path for an SVG icon.
    /// </summary>
    public static string GetPath(this SvgIcon icon)
        => $"./_content/ViciOne.Suite.Sdk.Client/svg/{icon.ToString().ToHyphenSeparated()}.svg";
}
