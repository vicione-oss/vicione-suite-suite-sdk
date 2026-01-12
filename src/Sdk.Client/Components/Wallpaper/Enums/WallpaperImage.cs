namespace Sdk.Client.Components.Wallpaper.Enums;

/// <summary>
/// Defines icon names for the Monochrome Icon set.
/// </summary>
public readonly record struct WallpaperImage
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static readonly WallpaperImage BlackAbstractTriangles = new(nameof(BlackAbstractTriangles));

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    private readonly string _name;

    internal WallpaperImage(string name)
        => _name = name;

    public string GetName()
        => _name;

    /// <inheritdoc cref="GetName"/>
    public override string ToString()
        => GetName();
}
