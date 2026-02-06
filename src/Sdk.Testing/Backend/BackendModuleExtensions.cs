using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Backend.Modules;
using Sdk.Instance;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="BackendModule"/> to facilitate integration testing.
/// </summary>
public static class BackendModuleExtensions
{
    extension<TModule>(TModule module) where TModule : BackendModule
    {
        private ServiceProvider TestRegisterServices(Action<ServiceCollection>? setup = null,
            Action<IBusRegistrationConfigurator>? busSetup = null)
        {
            var services = new ServiceCollection();
            services
                .AddConfiguration()
                .AddWorkspaceService<TModule>()
                .AddMvcBuilder()
                .AddEndpointRouteBuilder()
                .AddMassTransitConfigurators(busSetup)
                .AddLogging();

            setup?.Invoke(services);

            using var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var mvcBuilder = serviceProvider.GetRequiredService<IMvcBuilder>();
            var busRegistration = serviceProvider.GetRequiredService<IBusRegistrationConfigurator>();

            module.ConfigureServices(services, config, mvcBuilder);
            module.ConfigureMessageBus(busRegistration, InstanceType.Standalone);

            // todo
            // var endpointBuilder = serviceProvider.GetRequiredService<IEndpointRouteBuilder>();
            // module.MapEndpoints(endpointBuilder);

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Initializes a backend module for testing, setting up services and the application pipeline.
        /// </summary>
        /// <param name="services">Modify application builder</param>
        /// <param name="setup">Modify bus configuration</param>
        public ServiceProvider TestModuleInitialization(Action<ServiceCollection>? services = null,
            Action<IApplicationBuilder>? setup = null) => module.TestSagaModuleInitialization(services, setup);

        /// <summary>
        /// Initializes a backend module with saga support for testing, setting up services,
        /// the application pipeline, and the message bus.
        /// </summary>
        /// <param name="services">Modify DI container</param>
        /// <param name="setup">Modify application builder</param>
        /// <param name="busSetup">Modify bus configuration</param>
        public ServiceProvider TestSagaModuleInitialization(Action<ServiceCollection>? services = null,
            Action<IApplicationBuilder>? setup = null,
            Action<IBusRegistrationConfigurator>? busSetup = null)
        {
            var serviceProvider = module.TestRegisterServices(services, busSetup);

            var appMock = Substitute.For<IApplicationBuilder>();
            setup?.Invoke(appMock);

            appMock.ApplicationServices
                .Returns(serviceProvider);

            module.UseServices(appMock);

            return serviceProvider;
        }
    }
}
