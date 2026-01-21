using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for <see cref="ClientModule"/> to facilitate integration testing.
/// </summary>
public static class ClientModuleExtensions
{
    /// <summary>
    /// Initializes a client module for testing, setting up required client services and invoking the module's configuration logic.
    /// </summary>
    public static ServiceProvider TestModuleInitialization<TModule>(this TModule module, Action<ClientServiceConfigurator>? setup = null)
        where TModule : ClientModule
    {
        var services = new ServiceCollection();
        services
            .AddLogging()
            .AddClientServices(setup)
            .AddLocalization();

        module.Configure?.Invoke(services);

        return services.BuildServiceProvider();
    }
}
