using MassTransit;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Sdk.Backend.Modules;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure services for backend testing.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers an <see cref="IWorkspaceProvider{TModule}"/> substitute. Without <paramref name="setup"/>, its <c>Home</c>
        /// and <c>Cache</c> point to <c>home</c> and <c>cache</c> next to the calling assembly.
        /// </summary>
        public IServiceCollection AddWorkspaceService<TModule>(Action<IWorkspaceProvider<TModule>>? setup = null)
            where TModule : BackendModule
        {
            var workspace = Substitute.For<IWorkspaceProvider<TModule>>();
            services.AddSingleton(workspace);

            if (setup is null)
            {
                var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                if (string.IsNullOrEmpty(location))
                    return services;

                workspace.Home.Returns(Path.Combine(location, "home"));
                workspace.Cache.Returns(Path.Combine(location, "cache"));
            }
            else
            {
                setup.Invoke(workspace);
            }

            return services;
        }

        /// <summary>
        /// Registers an <see cref="IMvcBuilder"/> substitute whose <c>PartManager</c> is a real, empty part manager.
        /// </summary>
        public IServiceCollection AddMvcBuilder(Action<IMvcBuilder>? setup = null)
        {
            var mvcBuilder = Substitute.For<IMvcBuilder>();
            mvcBuilder.PartManager.Returns(new Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPartManager());
            services.AddSingleton(mvcBuilder);

            setup?.Invoke(mvcBuilder);

            return services;
        }

        /// <summary>
        /// Registers an <see cref="IConfiguration"/> built from the <see cref="TestConfig"/> defaults
        /// plus <paramref name="customSettings"/>.
        /// </summary>
        public IServiceCollection AddConfiguration(Dictionary<string, string?>? customSettings = null)
        {
            services.AddSingleton(new TestConfig().AddCustomSettings(customSettings).BuildConfiguration());

            return services;
        }

        /// <summary>
        /// Registers an <see cref="IConfiguration"/> built from <paramref name="config"/>; later changes to it are not seen.
        /// </summary>
        public IServiceCollection AddConfiguration(TestConfig config)
        {
            services.AddSingleton(config.BuildConfiguration());

            return services;
        }

        /// <summary>
        /// Registers an <see cref="IEndpointRouteBuilder"/> substitute.
        /// </summary>
        public IServiceCollection AddEndpointRouteBuilder(Action<IEndpointRouteBuilder>? setup = null)
        {
            var endpointBuilder = Substitute.For<IEndpointRouteBuilder>();
            services.AddSingleton(endpointBuilder);

            setup?.Invoke(endpointBuilder);

            return services;
        }

        /// <summary>
        /// Registers <see cref="IBusRegistrationConfigurator"/> and <see cref="ISagaRegistrationConfigurator"/> substitutes;
        /// <paramref name="setup"/> receives the bus configurator.
        /// </summary>
        public IServiceCollection AddMassTransitConfigurators(Action<IBusRegistrationConfigurator>? setup = null)
        {
            var busRegistration = Substitute.For<IBusRegistrationConfigurator>();
            var sagaRegistration = Substitute.For<ISagaRegistrationConfigurator>();

            services.AddSingleton(busRegistration);
            services.AddSingleton(sagaRegistration);

            setup?.Invoke(busRegistration);

            return services;
        }

        /// <summary>
        /// Registers <paramref name="config"/> as <see cref="IConfiguration"/>, removing the first existing registration if any.
        /// </summary>
        public IServiceCollection ReplaceConfiguration(IConfiguration config)
        {
            var configDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IConfiguration));
            if (configDescriptor is not null)
                services.Remove(configDescriptor);

            services.AddSingleton(config);

            return services;
        }
    }
}
