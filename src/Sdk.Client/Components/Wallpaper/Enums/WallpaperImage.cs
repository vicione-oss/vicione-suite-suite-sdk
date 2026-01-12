namespace Sdk.Client.Components.Wallpaper.Enums;

/// <summary>
/// Represents a specific wallpaper image available in the application.
/// </summary>
public readonly record struct WallpaperImage
{
    /// <summary>
    /// Represents the 'Black Abstract Triangles' wallpaper image.
    /// </summary>
    public static readonly WallpaperImage BlackAbstractTriangles = new(nameof(BlackAbstractTriangles));

    private readonly string _name;

    internal WallpaperImage(string name)
        => _name = name;

    /// <summary>
    /// Gets the name of the wallpaper image.
    /// </summary>
    public string GetName()
        => _name;

    /// <inheritdoc cref="GetName"/>
    public override string ToString()
        => GetName();
}
