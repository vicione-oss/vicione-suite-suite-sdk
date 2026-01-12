using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Modules;

namespace Sdk.Authorization.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure module authorization and features.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers services required for module authorization.
    /// </summary>
    public static IServiceCollection AddSdkAuthorization(this IServiceCollection services)
    {
        services.TryAddTransient<IModuleAuthorizationClaimParser, ModuleAuthorizationClaimParser>();
        return services;
    }

    /// <summary>
    /// Adds a feature to the module.
    /// </summary>
    /// <remarks>This is required to authorize functionality based on the feature name.</remarks>
    public static IServiceCollection AddModuleFeature(this IServiceCollection services, Func<IServiceProvider, IModuleFeature> featureFactory)
        => services.AddTransient(featureFactory);

    /// <summary>
    /// Registers a module feature for a specific module type with a name and description.
    /// </summary>
    /// <remarks>This is required to authorize functionality based on the feature name.</remarks>
    public static IServiceCollection AddModuleFeature<TModule>(this IServiceCollection services, string featureName, string description)
        where TModule : IModule
        => services.AddModuleFeature(_ => new ModuleFeature<TModule>(featureName, description));
}
