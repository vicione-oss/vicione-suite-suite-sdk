using Microsoft.AspNetCore.Components;

namespace Sdk.Client.NavTiles.Components;

/// <summary>
/// Marker interface to make NavTile component types discoverable by <see cref="Services.NavTileRegistry{TModule}"/>.
/// </summary>
public interface INavTile : IComponent;
