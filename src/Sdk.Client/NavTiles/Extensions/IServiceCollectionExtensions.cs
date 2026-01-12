using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;
using Sdk.Client.NavTiles.Services;

namespace Sdk.Client.NavTiles.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds navigation tile services for a specific module.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddNavTiles<TClientModule>(this IServiceCollection services)
        where TClientModule : class, IClientModule
    {
        services.AddScoped<INavTileRegistry<TClientModule>, NavTileRegistry<TClientModule>>();

        // additionally register interface with IClientModule to allow resolving all registries into an IEnumerable<>
        services.AddScoped<INavTileRegistry<IClientModule>>(serviceProvider => serviceProvider.GetRequiredService<INavTileRegistry<TClientModule>>());

        return services;
    }
}
