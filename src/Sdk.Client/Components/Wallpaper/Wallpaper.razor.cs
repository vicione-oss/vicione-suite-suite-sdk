using Microsoft.AspNetCore.Components;
using Sdk.Client.Components.Wallpaper.Enums;

namespace Sdk.Client.Components.Wallpaper;

/// <summary>
/// Positions itself in a container absolutely and renders the specified image
/// </summary>
public sealed partial class Wallpaper : ComponentBase
{
    private readonly Uri _defaultBaseUri = new("./_content/ViciOne.Suite.Sdk.Client/wallpapers", UriKind.Relative);

    /// <summary>
    /// Wallpaper image to render
    /// </summary>
    [Parameter, EditorRequired]
    public WallpaperImage Image { get; set; }

    /// <summary>
    /// Optional base URI used for resolving the image URL
    /// </summary>
    [Parameter]
    public Uri? BaseUri { get; set; }
}
