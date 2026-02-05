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
        public IServiceCollection AddModuleFeature(Func<IServiceProvider, IModuleFeature> featureFactory)
            => services.AddTransient(featureFactory);

        /// <summary>
        /// Registers a module feature for a specific module type with a name and description.
        /// </summary>
        /// <remarks>This is required to authorize functionality based on the feature name.</remarks>
        public IServiceCollection AddModuleFeature<TModule>(string featureName, string description)
            where TModule : IModule
            => services.AddModuleFeature(_ => new ModuleFeature<TModule>(featureName, description));
    }
}
