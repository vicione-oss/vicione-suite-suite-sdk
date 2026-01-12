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
    /// Identifier for uniquely identifying the navigation tile in <see cref="INavTileRegistry{TClientModule}"/> operations.
    /// </summary>
    /// <remarks>
    /// The default value is a random <see cref="Guid"/> string.
    /// </remarks>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Horizontal span of the navigation tile
    /// </summary>
    public NavTileSpan HorizontalSpan { get; init; }

    /// <summary>
    /// Specifies whether the navigation tile should be enabled or not.
    /// </summary>
    /// <remarks>
    /// If the value is set to <see langword="true"/> then certain visual effects are disabled and no clicks are registered.
    /// The default value is <see langword="true"/>.
    /// </remarks>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Sets the link target used in the default navigation handling executed when the navigation tile is clicked.
    /// </summary>
    public string? LinkTarget { get; init; }

    /// <summary>
    /// Sets the group of the navigation tile.
    /// </summary>
    public NavTileGroup Group { get; init; } = NavTileGroup.Applications;
}
