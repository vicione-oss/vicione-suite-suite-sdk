using Microsoft.AspNetCore.Authorization;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Services;

public class NavTileRegistryItem
{
    public required string Id { get; init; }
    public required Type ComponentType { get; init; }
    public required NavTileState State { get; init; }
    public required NavTileGroup Group { get; init; }

    /// <summary>
    /// Optional authorization requirement, otherwise <see langword="null" /> to skip authorization
    /// </summary>
    public IAuthorizationRequirement? AuthorizationRequirement { get; init; }
}
