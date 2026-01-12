using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Modules;

namespace Sdk.Authorization.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IModuleAuthorizationClaimParser"/> as singleton to <paramref name="services"/>.
    /// </summary>
    public static IServiceCollection AddSdkAuthorization(this IServiceCollection services)
    {
        services.TryAddTransient<IModuleAuthorizationClaimParser, ModuleAuthorizationClaimParser>();
        return services;
    }

    /// <summary>
    ///     Adds a feature to the module. This is required to authorize functionality based on the feature name.
    /// </summary>
    /// <remarks>Changing the feature name of an existing feature will create new claims and is not recommended</remarks>
    public static IServiceCollection AddModuleFeature(this IServiceCollection services, Func<IServiceProvider, IModuleFeature> featureFactory)
        => services.AddTransient(featureFactory);

    /// <summary>
    ///     Adds a feature to the module. This is required to authorize functionality based on the feature name.
    /// </summary>
    /// <remarks>Changing the feature name of an existing feature will create new claims and is not recommended</remarks>
    public static IServiceCollection AddModuleFeature<TModule>(this IServiceCollection services, string featureName, string description)
        where TModule : IModule
        => services.AddModuleFeature(_ => new ModuleFeature<TModule>(featureName, description));
}
