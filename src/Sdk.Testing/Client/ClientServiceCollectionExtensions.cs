using Sdk.Client.Modules;
using Sdk.Client.Modules.Localization;
using Sdk.Instance;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure services for client-side testing.
/// </summary>
public static class ClientServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds a collection of common client-side services, mostly as mocks or fakes, to the service collection for testing.
        /// </summary>
        public IServiceCollection AddClientServices(Action<ClientServiceConfigurator>? configurator)
        {
            var config = new ClientServiceConfigurator(services);

            services
                .AddSingleton(config.ActiveNotificationElementPolicy)
                .AddSingleton(config.ClientMediator)
                .AddSingleton(config.ConnectionService)
                .AddSingleton(config.InstanceInformationProvider)
                .AddSingleton(config.JsInterop)
                .AddSingleton(config.Layout)
                .AddSingleton(config.MessageBanner)
                .AddSingleton(config.StringLocalizerFactory)
                .AddSingleton(config.ControlPanelRegistryFactory)
                .AddSingleton(config.NavTileRegistryFactory)
                .AddSingleton(config.NotificationElementRegistryFactory);

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

        /// <summary>
        /// Adds a singleton <see cref="HttpClient"/> to the service collection that is configured to return a specific response object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2000:Objekte verwerfen, bevor Bereich verloren geht",
            Justification = "Rückgabewerte für Testattrappe")]
        public IServiceCollection AddHttpClient(object responseObject, Uri? baseUri)
        {
            var httpClient = HttpClientFactory.GetHttpClientWithResponse(responseObject, baseUri ?? new Uri("http://localhost"));
            services.AddSingleton(httpClient);

            return services;
        }

        /// <summary>
        /// Adds a mocked <see cref="IClientModuleLocalizer{TClientModule}"/> to the service collection.
        /// </summary>
        public IServiceCollection AddLocalization<TClientModule>()
            where TClientModule : class, IClientModule
        {
            var localizer = Substitute.For<IClientModuleLocalizer<TClientModule>>();

            localizer.GetTitle().Returns("Test client module");
            localizer.GetDescription().Returns("Test client module description");

            services.AddSingleton(localizer);

            return services;
        }
    }
}
