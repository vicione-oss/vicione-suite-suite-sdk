using Microsoft.JSInterop;
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
        /// Registers the substitutes of a <see cref="ClientServiceConfigurator"/> that <paramref name="configurator"/> can adjust.
        /// </summary>
        /// <remarks>
        /// The substitutes are registered before <paramref name="configurator"/> runs; the switches such as
        /// <see cref="ClientServiceConfigurator.UseNavigationManager"/> are evaluated after it.
        /// </remarks>
        public IServiceCollection AddClientServices(Action<ClientServiceConfigurator>? configurator)
        {
            var config = new ClientServiceConfigurator(services);

            // NSubstitute auto-generates a non-null IJSObjectReference from IncludeModuleScript by default.
            // Methods like InvokeConstructorAsync on that auto-generated mock return null, which causes NREs
            // when components call InvokeVoidAsync on the result. Return null explicitly so that components'
            // JS interop null guards prevent entering those code paths during tests.
            config.JsInterop.IncludeModuleScript(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IJSObjectReference?>(null));

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
        /// Registers an <see cref="HttpClient"/> that answers every request with 200 and
        /// <paramref name="responseObject"/> as camel-case JSON.
        /// </summary>
        /// <param name="responseObject">The response body; an empty string yields an empty body.</param>
        /// <param name="baseUri">The client's base address; <see langword="null"/> means <c>http://localhost</c>.</param>
        public IServiceCollection AddHttpClient(object responseObject, Uri? baseUri)
        {
            services.AddSingleton(_ => HttpClientFactory.GetHttpClientWithResponse(responseObject, baseUri ?? new Uri("http://localhost")));

            return services;
        }

        /// <summary>
        /// Registers an <see cref="IClientModuleLocalizer{TClientModule}"/> substitute with a fixed title and description.
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
