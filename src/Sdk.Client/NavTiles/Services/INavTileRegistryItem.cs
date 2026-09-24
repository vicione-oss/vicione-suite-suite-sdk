using Microsoft.AspNetCore.Authorization;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Services;

/// <summary>
/// A navigation tile registered in an <see cref="INavTileRegistry{TClientModule}"/>.
/// </summary>
public interface INavTileRegistryItem
{
    /// <summary>
    /// Gets the tile's ID.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the component type that renders the tile.
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Gets the tile's state.
    /// </summary>
    NavTileState State { get; }

    /// <summary>
    /// Gets the tile's group.
    /// </summary>
    NavTileGroup Group { get; }

    /// <summary>
    /// Gets the requirement to see the tile; <see langword="null"/> if everyone may.
    /// </summary>
    IAuthorizationRequirement? AuthorizationRequirement { get; }
}
