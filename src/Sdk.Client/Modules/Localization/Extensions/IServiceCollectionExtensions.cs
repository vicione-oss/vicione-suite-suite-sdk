using Microsoft.Extensions.DependencyInjection;

namespace Sdk.Client.Modules.Localization.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds services required for client module localization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddLocalization<TClientModule, TClientModuleLocalizer>(this IServiceCollection services)
        where TClientModule : class, IClientModule
        where TClientModuleLocalizer : class, IClientModuleLocalizer<TClientModule>
    {
        services.AddSingleton<IClientModuleLocalizer<TClientModule>, TClientModuleLocalizer>();

        // additionally register interface with IClientModule to allow resolving all registries into an IEnumerable<>
        services.AddSingleton<IClientModuleLocalizer<IClientModule>>(serviceProvider => serviceProvider.GetRequiredService<IClientModuleLocalizer<TClientModule>>());

        return services;
    }
}