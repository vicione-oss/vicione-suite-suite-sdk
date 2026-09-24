using Microsoft.Extensions.DependencyInjection;

namespace Sdk.Client.Modules.Localization.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register localization services for client modules.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TClientModuleLocalizer"/> as the module's singleton localizer.
    /// </summary>
    public static IServiceCollection AddLocalization<TClientModule, TClientModuleLocalizer>(this IServiceCollection services)
        where TClientModule : class, IClientModule
        where TClientModuleLocalizer : class, IClientModuleLocalizer<TClientModule>
    {
        services.AddSingleton<IClientModuleLocalizer<TClientModule>, TClientModuleLocalizer>();

        // The IClientModule view lets every module's localizer resolve through one IEnumerable<IClientModuleLocalizer<IClientModule>>.
        services.AddSingleton<IClientModuleLocalizer<IClientModule>>(serviceProvider => serviceProvider.GetRequiredService<IClientModuleLocalizer<TClientModule>>());

        return services;
    }
}
