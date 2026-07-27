using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Contracts;
using Sdk.Modules;

namespace Sdk.Client.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/> to register client-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds a stream upload handler for the specified context type, while allowing for optional configuration of the handler options.
        /// </summary>
        /// <typeparam name="TMarker">Marker type to match the upload to the correct handler</typeparam>
        /// <param name="moduleId">Used to identify the correct backend module</param>
        /// <param name="configureOptions">Allows the customization of the handler options</param>
        /// <returns></returns>
        public IServiceCollection AddStreamUploadHandler<TMarker>(string moduleId, Action<StreamUploadHandlerOptions>? configureOptions = null)
        {
            _ = services.AddTransient((sp) =>
            {
                var factory = sp.GetRequiredService<IStreamUploadHandlerFactory>();
                var options = new StreamUploadHandlerOptions();

                if (configureOptions is not null)
                {
                    configureOptions(options);
                }

                return factory.CreateStreamUploadHandlerFromModuleId<TMarker>(options, moduleId);
            });

            return services;
        }

        /// <summary>
        /// Adds a stream upload handler for the specified context type, while allowing for optional configuration of the handler options.
        /// </summary>
        /// <typeparam name="TMarker">Marker type to match the upload to the correct handler</typeparam>
        /// <typeparam name="TModule">Used to identify the correct backend module</typeparam>
        /// <param name="configureOptions">Allows the customization of the handler options</param>
        /// <returns></returns>
        public IServiceCollection AddStreamUploadHandler<TMarker, TModule>(Action<StreamUploadHandlerOptions>? configureOptions = null) where TModule : IModule
        {
            _ = services.AddTransient((sp) =>
            {
                var factory = sp.GetRequiredService<IStreamUploadHandlerFactory>();
                var options = new StreamUploadHandlerOptions();

                if (configureOptions is not null)
                {
                    configureOptions(options);
                }

                return factory.CreateStreamUploadHandler<TMarker, TModule>(options);
            });

            return services;
        }
    }
}
