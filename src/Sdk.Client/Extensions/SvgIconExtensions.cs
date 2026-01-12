using Sdk.Client.Enums;

namespace Sdk.Client.Extensions;

public static class SvgIconExtensions
{
    /// <summary>
    /// Get the relative url path to svg icon
    /// </summary>
    public static string GetPath(this SvgIcon icon)
        => $"./_content/ViciOne.Suite.Sdk.Client/svg/{icon.ToString().ToHyphenSeparated()}.svg";
}
