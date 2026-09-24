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
        /// Registers a transient <see cref="IStreamUploadHandler{T}"/> of <typeparamref name="TMarker"/> that uploads into the
        /// workspace of module <paramref name="moduleId"/>.
        /// </summary>
        /// <typeparam name="TMarker">Ties the handler to one upload control.</typeparam>
        /// <param name="moduleId">The backend module whose workspace receives the files.</param>
        /// <param name="configureOptions">Adjusts the handler options; <see langword="null"/> keeps the defaults.</param>
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
        /// Registers a transient <see cref="IStreamUploadHandler{T}"/> of <typeparamref name="TMarker"/> that uploads into the
        /// workspace of <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TMarker">Ties the handler to one upload control.</typeparam>
        /// <typeparam name="TModule">The backend module whose workspace receives the files.</typeparam>
        /// <param name="configureOptions">Adjusts the handler options; <see langword="null"/> keeps the defaults.</param>
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
