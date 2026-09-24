using Sdk.Client.Components.Wallpaper.Enums;

namespace Sdk.Client.Components.Wallpaper;

/// <summary>
/// Renders a wallpaper image, positioned absolutely within its container.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed partial class Wallpaper : ComponentBase
{
    private readonly Uri _defaultBaseUri = new("./_content/ViciOne.Suite.Sdk.Client/wallpapers", UriKind.Relative);

    /// <summary>
    /// Gets or sets the image to render.
    /// </summary>
    [Parameter, EditorRequired]
    public WallpaperImage Image { get; set; }

    /// <summary>
    /// Gets or sets the directory the image is loaded from; <see langword="null"/> means the wallpapers shipped with this package.
    /// </summary>
    [Parameter]
    public Uri? BaseUri { get; set; }
}
