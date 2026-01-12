using Microsoft.AspNetCore.Authorization;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Services;

/// <summary>
/// Item to register a navigation tile in a <see cref="INavTileRegistry{TClientModule}"/>
/// </summary>
public interface INavTileRegistryItem
{
    /// <summary>
    /// Identifier that uniquely identifies the navigation tile
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Component type that implements the navigation tile
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// State of the navigation tile
    /// </summary>
    NavTileState State { get; }

    /// <summary>
    /// Group of the navigation tile
    /// </summary>
    NavTileGroup Group { get; }

    /// <summary>
    /// Optional authorization requirement, otherwise <see langword="null" /> to skip authorization
    /// </summary>
    IAuthorizationRequirement? AuthorizationRequirement { get; }
}
