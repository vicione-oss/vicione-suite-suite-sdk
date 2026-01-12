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
    /// Adds navigation tile services for a specific module.
    /// </summary>
    /// <typeparam name="TClientModule">The type of the client module for which the services are registered.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
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
            // Additionally register interface with IClientModule to allow resolving all registries into an IEnumerable<>
            services.AddScoped<INavTileRegistry<IClientModule>>(serviceProvider => serviceProvider.GetRequiredService<INavTileRegistry<TClientModule>>());
        }

        return services;
    }
}
