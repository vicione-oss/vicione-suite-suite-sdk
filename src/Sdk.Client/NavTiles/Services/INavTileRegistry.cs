using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sdk.Client.Modules;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Services;

/// <summary>
/// Registry for navigation tiles
/// </summary>
public interface INavTileRegistry<out TClientModule> : IEnumerable<INavTileRegistryItem>
    where TClientModule : IClientModule
{
    /// <summary>
    /// Raised when the registry was changed.
    /// </summary>
    event Action Changed;

    /// <summary>
    /// Registers a navigation tile in the registry.
    /// </summary>
    /// <param name="id">Identifier for uniquely identifying the navigation tile</param>
    /// <param name="horizontalSpan">Horizontal span of the navigation tile</param>
    /// <param name="enabled">Specifies whether the navigation tile should be enabled or not</param>
    /// <param name="linkTarget">Link target used in the default navigation handling executed when the navigation tile is clicked</param>
    /// <param name="group">Group of the navigation tile</param>
    /// <param name="authorizationRequirement">Optional authorization requirement, otherwise <see langword="null" /> to skip authorization</param>
    INavTileRegistryItem Add<T>(string id, NavTileSpan horizontalSpan = NavTileSpan.One, bool enabled = true,
        string? linkTarget = null, NavTileGroup group = NavTileGroup.Applications, IAuthorizationRequirement? authorizationRequirement = null)
            where T : ComponentBase, INavTile;

    /// <summary>
    /// Removes the navigation tile with the given <paramref name="id"/> from the registry.
    /// </summary>
    /// <returns><see langword="true"/> when the navigation tile was removed, otherwise <see langword="false"/></returns>
    bool Remove(string id);
}
