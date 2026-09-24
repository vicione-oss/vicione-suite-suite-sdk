using Sdk.Client.Components.Wallpaper.Enums;
using Sdk.Client.Extensions;

namespace Sdk.Client.Components.Wallpaper.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WallpaperImage"/>.
/// </summary>
public static class WallpaperImageExtensions
{
    /// <summary>
    /// Returns <c>{baseUri}/{image-name}.svg</c>, with the image name hyphen-separated, e.g. <c>black-abstract-triangles.svg</c>.
    /// </summary>
    public static string GetPath(this WallpaperImage image, Uri baseUri)
        => new Uri($"{baseUri}/{image.ToString().ToHyphenSeparated()}.svg", baseUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative).ToString();
}
