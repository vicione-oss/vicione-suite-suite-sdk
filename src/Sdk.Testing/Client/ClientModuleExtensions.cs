using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;

namespace Sdk.Testing.Client;

public static class ClientModuleExtensions
{
    /// <summary>
    /// Add client services and invokes ConfigureServices on module. 
    /// Builds service provider afterwards
    /// </summary>
    /// <param name="module">modify service collection</param>
    /// <param name="setup">modify application builder</param>
    /// <param name="hostingModel">hosting model parameter to use in module service registration</param>
    /// <typeparam name="TModule">module to initialize</typeparam>
    /// <returns></returns>
    public static ServiceProvider TestModuleInitialization<TModule>(this TModule module, Action<ClientServiceConfigurator>? setup = null, HostingModel hostingModel = HostingModel.BlazorServer)
        where TModule : ClientModule
    {
        var services = new ServiceCollection();
        services
            .AddLogging()
            .AddClientServices(setup)
            .AddLocalization();

        module.ConfigureServices?.Invoke(services, hostingModel);

        return services.BuildServiceProvider();
    }
}
