using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Client.Modules;
using Sdk.Client.NavTiles.Services;

namespace Sdk.Client.NavTiles.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register navigation tile services.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="INavTileRegistry{TClientModule}"/>, and <see cref="INavTileRegistry{TClientModule}"/> of
    /// <see cref="IClientModule"/> for it, once per module.
    /// </summary>
    public static IServiceCollection AddNavTiles<TClientModule>(this IServiceCollection services)
        where TClientModule : class, IClientModule
    {
        var serviceCountBefore = services.Count;

        services.TryAddScoped(static serviceProvider =>
        {
            var registryFactory = serviceProvider.GetRequiredService<INavTileRegistryFactory>();
            var registry = registryFactory.CreateNavTileRegistry<TClientModule>();

            return registry;
        });

        if (serviceCountBefore < services.Count)
        {
            // The IClientModule view lets every module's registry resolve through one IEnumerable<INavTileRegistry<IClientModule>>.
            services.AddScoped<INavTileRegistry<IClientModule>>(serviceProvider => serviceProvider.GetRequiredService<INavTileRegistry<TClientModule>>());
        }

        return services;
    }
}
