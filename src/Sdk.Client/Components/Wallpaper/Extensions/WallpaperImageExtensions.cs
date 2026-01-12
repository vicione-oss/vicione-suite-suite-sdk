using Sdk.Client.Components.Wallpaper.Enums;
using Sdk.Client.Extensions;

namespace Sdk.Client.Components.Wallpaper.Extensions;

/// <summary>
/// Extension methods for <see cref="WallpaperImage"/>
/// </summary>
public static class WallpaperImageExtensions
{
    /// <returns>Path with the pattern {<paramref name="baseUri"/>}/{<paramref name="image"/>}.svg whereas image is hyphen-separated</returns>
    public static string GetPath(this WallpaperImage image, Uri baseUri)
        => new Uri($"{baseUri}/{image.ToString().ToHyphenSeparated()}.svg", baseUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative).ToString();
}
