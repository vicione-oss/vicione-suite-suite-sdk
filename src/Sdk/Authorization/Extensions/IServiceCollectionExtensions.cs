using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Modules;

namespace Sdk.Authorization.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure module authorization and features.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers services required for module authorization.
        /// </summary>
        public IServiceCollection AddSdkAuthorization()
        {
            services.TryAddTransient<IModuleAuthorizationClaimParser, ModuleAuthorizationClaimParser>();
            return services;
        }

        /// <summary>
        /// Adds a feature to the module.
        /// </summary>
        /// <remarks>This is required to authorize functionality based on the feature name.</remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the feature is resolved, if its name contains <c>_</c>; see <see cref="AccessLevelPolicyParser"/>.
        /// </exception>
        public IServiceCollection AddModuleFeature(Func<IServiceProvider, IModuleFeature> featureFactory)
            => services.AddTransient(serviceProvider =>
            {
                var feature = featureFactory(serviceProvider);
                ValidateFeatureName(feature.Name, nameof(featureFactory));
                return feature;
            });

        /// <summary>
        /// Registers a module feature for a specific module type with a name and description.
        /// </summary>
        /// <remarks>This is required to authorize functionality based on the feature name.</remarks>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="featureName"/> contains <c>_</c>; see <see cref="AccessLevelPolicyParser"/>.
        /// </exception>
        public IServiceCollection AddModuleFeature<TModule>(string featureName, string description)
            where TModule : IModule
        {
            ValidateFeatureName(featureName, nameof(featureName));
            return services.AddModuleFeature(_ => new ModuleFeature<TModule>(featureName, description));
        }
    }

    // A policy name joins its parts with '_' (see ModulePolicyProvider), so a feature name containing one could not be
    // parsed back and would be truncated by AccessLevelPolicyParser.
    private static void ValidateFeatureName(string featureName, string paramName)
    {
        if (featureName.Contains('_', StringComparison.Ordinal))
            throw new ArgumentException($"Feature name '{featureName}' must not contain '_', the separator of policy names.", paramName);
    }
}
