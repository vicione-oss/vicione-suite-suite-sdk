using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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
        /// Adds a mocked <see cref="IWorkspaceProvider{TModule}"/> to the service collection.
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
        /// Adds a mocked <see cref="IMvcBuilder"/> to the service collection.
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
        /// Adds a test-specific <see cref="IConfiguration"/> to the service collection.
        /// </summary>
        public IServiceCollection AddConfiguration(Dictionary<string, string?>? customSettings = null)
        {
            services.AddSingleton(new TestConfig().AddCustomSettings(customSettings).BuildConfiguration());

            return services;
        }

        /// <summary>
        /// Adds a test-specific <see cref="IConfiguration"/>, built from the provided <see cref="TestConfig"/>, to the service collection.
        /// </summary>
        public IServiceCollection AddConfiguration(TestConfig config)
        {
            services.AddSingleton(config.BuildConfiguration());

            return services;
        }

        /// <summary>
        /// Adds a mocked <see cref="IEndpointRouteBuilder"/> to the service collection.
        /// </summary>
        public IServiceCollection AddEndpointRouteBuilder(Action<IEndpointRouteBuilder>? setup = null)
        {
            var endpointBuilder = Substitute.For<IEndpointRouteBuilder>();
            services.AddSingleton(endpointBuilder);

            setup?.Invoke(endpointBuilder);

            return services;
        }

        /// <summary>
        /// Adds mocked MassTransit configurators (<see cref="IBusRegistrationConfigurator"/> and <see cref="ISagaRegistrationConfigurator"/>) to the service collection.
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
        /// Replaces any existing <see cref="IConfiguration"/> registration in the service collection with the provided instance.
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
