using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Client.Modules;
using Sdk.Client.Modules.Localization;
using Sdk.Instance;

namespace Sdk.Testing.Client;

public static class ClientServiceCollectionExtensions
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, Action<ClientServiceConfigurator>? configurator)
    {
        var config = new ClientServiceConfigurator(services);

        services
            .AddSingleton(config.ActiveNotificationElementPolicy)
            .AddSingleton(config.ClientMediator)
            .AddSingleton(config.ClientModuleService)
            .AddSingleton(config.ControlPanelService)
            .AddSingleton(config.ConnectionService)
            .AddSingleton(config.InstanceInformationProvider)
            .AddSingleton(config.JsInterop)
            .AddSingleton(config.Layout)
            .AddSingleton(config.MessageBanner)
            .AddSingleton(config.StringLocalizerFactory);

        var instanceInformation = Substitute.For<IInstanceInformation>();

        instanceInformation.Id
            .Returns(Guid.NewGuid());

        config.InstanceInformationProvider.Local
            .Returns(instanceInformation);

        configurator?.Invoke(config);

        if (config.UseNavigationManager)
            services.AddSingleton(config.NavigationManagerMock);

        if (config.UseSimpleJsMock)
            services.AddSingleton(config.JSRuntime);

        if (config.FakeAuthenticationStateProvider)
            services.AddSingleton(_ => ClientServiceFactory.CreateAuthenticationStateProvider());

        return services;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
        "CA2000:Objekte verwerfen, bevor Bereich verloren geht",
        Justification = "Rückgabewerte für Testattrappe")]
    public static IServiceCollection AddHttpClient(this IServiceCollection services, object responseObject, Uri? baseUri)
    {
        var httpClient = HttpClientFactory.GetHttpClientWithResponse(responseObject, baseUri ?? new Uri("http://localhost"));
        services.AddSingleton(httpClient);

        return services;
    }

    public static IServiceCollection AddLocalization<TClientModule>(this IServiceCollection services)
        where TClientModule : class, IClientModule
    {
        var localizer = Substitute.For<IClientModuleLocalizer<TClientModule>>();

        localizer.GetTitle().Returns("Test client module");
        localizer.GetDescription().Returns("Test client module description");

        services.AddSingleton(localizer);

        return services;
    }
}
