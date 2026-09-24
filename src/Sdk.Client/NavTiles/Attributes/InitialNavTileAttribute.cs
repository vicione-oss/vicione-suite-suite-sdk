using Sdk.Client.Modules;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;
using Sdk.Client.NavTiles.Services;

namespace Sdk.Client.NavTiles.Attributes;

/// <summary>
/// Marks a class implementing <see cref="INavTile"/> as an implementation that is automatically discovered
/// on instantiation of the class implementing <see cref="INavTileRegistry{TClientModule}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class InitialNavTileAttribute<TClientModule> : Attribute
    where TClientModule : IClientModule
{
    /// <summary>
    /// Gets or initializes the ID identifying the tile in <see cref="INavTileRegistry{TClientModule}"/> operations.
    /// Defaults to a random <see cref="Guid"/> string.
    /// </summary>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or initializes the tile's width. Defaults to <see cref="NavTileSpan.One"/>.
    /// </summary>
    public NavTileSpan HorizontalSpan { get; init; }

    /// <summary>
    /// Gets or initializes whether the tile is enabled; a disabled tile loses its hover effects and ignores clicks.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets or initializes the URL the tile navigates to when clicked, unless the tile overrides <see cref="NavTileBase.Click"/>.
    /// </summary>
    public string? LinkTarget { get; init; }

    /// <summary>
    /// Gets or initializes the tile's group. Defaults to <see cref="NavTileGroup.Applications"/>.
    /// </summary>
    public NavTileGroup Group { get; init; } = NavTileGroup.Applications;
}
