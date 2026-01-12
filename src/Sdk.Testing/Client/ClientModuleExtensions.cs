using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for <see cref="ClientModule"/> to facilitate integration testing.
/// </summary>
public static class ClientModuleExtensions
{
    /// <summary>
    /// Initializes a client module for testing with a specific hosting model. This method is obsolete.
    /// </summary>
    [Obsolete("Support of " + nameof(HostingModel) + " will be removed and therefore this method is obsolete. Use " + nameof(TestModuleInitialization) + " without 'hostingModel' parameter instead.")]
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
