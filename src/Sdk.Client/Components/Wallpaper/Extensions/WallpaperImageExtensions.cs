using Sdk.Client.Components.Wallpaper.Enums;
using Sdk.Client.Extensions;

namespace Sdk.Client.Components.Wallpaper.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WallpaperImage"/>.
/// </summary>
public static class WallpaperImageExtensions
{
    /// <summary>
    /// Constructs the full path for <paramref name="image"/> based on a <paramref name="baseUri"/>.
    /// </summary>
    public static string GetPath(this WallpaperImage image, Uri baseUri)
        => new Uri($"{baseUri}/{image.ToString().ToHyphenSeparated()}.svg", baseUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative).ToString();
}
