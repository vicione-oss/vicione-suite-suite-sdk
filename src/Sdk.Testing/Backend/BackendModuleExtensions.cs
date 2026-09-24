using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Sdk.Backend.Modules;
using Sdk.Backend.Persistence;
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
            services.AddSingleton<IModuleDbContextRegistrar, TestModuleDbContextRegistrar>();
            setup?.Invoke(services);

            using var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var mvcBuilder = serviceProvider.GetRequiredService<IMvcBuilder>();
            var busRegistration = serviceProvider.GetRequiredService<IBusRegistrationConfigurator>();

            module.ConfigureServices(services, config, mvcBuilder);
            module.ConfigureMessageBus(busRegistration, InstanceType.Standalone);

            // todo: call module.MapEndpoints with the registered IEndpointRouteBuilder substitute.

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Runs the module's service, message-bus and pipeline configuration against test doubles and returns the resulting provider.
        /// </summary>
        /// <param name="services">Adjusts the service collection before the module configures it.</param>
        /// <param name="setup">Configures the <see cref="IApplicationBuilder"/> substitute passed to <c>UseServices</c>.</param>
        public ServiceProvider TestModuleInitialization(Action<ServiceCollection>? services = null,
            Action<IApplicationBuilder>? setup = null) => module.TestSagaModuleInitialization(services, setup);

        /// <summary>
        /// Like <see cref="TestModuleInitialization"/>, but also exposes the <see cref="IBusRegistrationConfigurator"/> substitute,
        /// e.g. to register sagas.
        /// </summary>
        /// <param name="services">Adjusts the service collection before the module configures it.</param>
        /// <param name="setup">Configures the <see cref="IApplicationBuilder"/> substitute passed to <c>UseServices</c>.</param>
        /// <param name="busSetup">Configures the <see cref="IBusRegistrationConfigurator"/> substitute before the module does.</param>
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
