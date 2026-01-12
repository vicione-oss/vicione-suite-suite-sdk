using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Backend.Modules;
using Sdk.Instance;

namespace Sdk.Testing.Backend;

public static class BackendModuleExtensions
{
    private static ServiceProvider TestRegisterServices<TModule>(this TModule module,
        Action<ServiceCollection>? setup = null,
        Action<IBusRegistrationConfigurator>? busSetup = null)
        where TModule : BackendModule
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
    /// 
    /// </summary>
    /// <param name="module">module instance that should get tested</param>
    /// <param name="services">modify application builder</param>
    /// <param name="setup">modify bus configuration</param>
    /// <typeparam name="TModule">type of tested module</typeparam>
    /// <returns></returns>
    public static ServiceProvider TestModuleInitialization<TModule>(this TModule module,
        Action<ServiceCollection>? services = null,
        Action<IApplicationBuilder>? setup = null)
        where TModule : BackendModule
        => module.TestSagaModuleInitialization(services, setup);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="module">module instance that should get tested</param>
    /// <param name="services">modify service collection</param>
    /// <param name="setup">modify application builder</param>
    /// <param name="busSetup">modify bus configuration</param>
    /// <typeparam name="TModule">type of tested module</typeparam>
    /// <returns></returns>
    public static ServiceProvider TestSagaModuleInitialization<TModule>(this TModule module,
        Action<ServiceCollection>? services = null,
        Action<IApplicationBuilder>? setup = null,
        Action<IBusRegistrationConfigurator>? busSetup = null)
        where TModule : BackendModule
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
