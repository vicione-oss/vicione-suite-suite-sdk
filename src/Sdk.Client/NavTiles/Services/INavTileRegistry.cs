using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Interfaces;
using Sdk.Client.Modules;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Services;

/// <summary>
/// Holds a module's navigation tiles.
/// </summary>
public interface INavTileRegistry<out TClientModule> : IEnumerable<INavTileRegistryItem>, IHasUpdateLock
    where TClientModule : IClientModule
{
    /// <summary>
    /// Raised when the registry was changed.
    /// </summary>
    event Action Changed;

    /// <summary>
    /// Registers a navigation tile in the registry.
    /// </summary>
    /// <param name="id">The ID identifying the tile in later operations.</param>
    /// <param name="horizontalSpan">The tile's width.</param>
    /// <param name="enabled">Whether the tile is enabled; a disabled tile ignores clicks.</param>
    /// <param name="linkTarget">The URL <see cref="NavTileBase.Click"/> navigates to; <see langword="null"/> for none.</param>
    /// <param name="group">The tile's group.</param>
    /// <param name="authorizationRequirement">The requirement to see the tile; <see langword="null"/> skips authorization.</param>
    INavTileRegistryItem Add<T>(string id, NavTileSpan horizontalSpan = NavTileSpan.One, bool enabled = true,
        string? linkTarget = null, NavTileGroup group = NavTileGroup.Applications, IAuthorizationRequirement? authorizationRequirement = null)
            where T : ComponentBase, INavTile;

    /// <summary>
    /// Removes the navigation tile with the given <paramref name="id"/> from the registry.
    /// </summary>
    /// <returns><see langword="true"/> if the tile was removed.</returns>
    bool Remove(string id);
}
