using Sdk.Client.Modules;

namespace Sdk.Client.NavTiles.Services;

/// <summary>
/// Defines a factory for creating navigation tile registries.
/// </summary>
/// <remarks>
/// This interface is intended for internal framework use only and should not be used directly by consumer applications.
/// </remarks>
public interface INavTileRegistryFactory
{
    /// <summary>
    /// Creates a new navigation tile registry associated with <typeparamref name="TClientModule"/>.
    /// </summary>
    INavTileRegistry<TClientModule> CreateNavTileRegistry<TClientModule>() where TClientModule : class, IClientModule;
}
