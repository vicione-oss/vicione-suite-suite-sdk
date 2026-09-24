using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Infrastructure;
using Sdk.Client.NavTiles.Services;
using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Services;
using Sdk.Instance;

namespace Sdk.Testing.Client;

/// <summary>
/// Holds the NSubstitute substitutes and switches that <see cref="ClientServiceCollectionExtensions.AddClientServices"/> registers;
/// configure the substitutes inside its callback.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ClientServiceConfigurator(IServiceCollection services)
{
    /// <summary>
    /// Gets the service collection the substitutes are registered in.
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// Gets the substitute registered as <see cref="IStringLocalizerFactory"/>.
    /// </summary>
    public IStringLocalizerFactory StringLocalizerFactory { get; } = Substitute.For<IStringLocalizerFactory>();

    /// <summary>
    /// Gets or sets whether <see cref="JSRuntime"/> replaces bUnit's own <see cref="IJSRuntime"/>. Defaults to false.
    /// </summary>
    public bool UseSimpleJsMock { get; set; }

    /// <summary>
    /// Gets or sets whether an <see cref="AuthenticationStateProvider"/> with a fixed, authenticated Administrator is registered.
    /// Defaults to true.
    /// </summary>
    public bool FakeAuthenticationStateProvider { get; set; } = true;

    /// <summary>
    /// Gets the substitute registered as <see cref="IConnectionService"/>.
    /// </summary>
    public IConnectionService ConnectionService { get; } = Substitute.For<IConnectionService>();

    /// <summary>
    /// Gets the substitute registered as <see cref="ILayoutService"/>.
    /// </summary>
    public ILayoutService Layout { get; } = Substitute.For<ILayoutService>();

    /// <summary>
    /// Gets the substitute registered as <see cref="IJsInterop"/>; <c>IncludeModuleScript</c> returns <see langword="null"/>
    /// so components skip their JavaScript code paths.
    /// </summary>
    public IJsInterop JsInterop { get; } = Substitute.For<IJsInterop>();

    /// <summary>
    /// Gets the substitute registered as <see cref="IInstanceInformationProvider"/>; its <c>Local</c> instance has a random ID.
    /// </summary>
    public IInstanceInformationProvider InstanceInformationProvider { get; } = Substitute.For<IInstanceInformationProvider>();

    /// <summary>
    /// Gets the substitute registered as <see cref="IUiMediator"/>.
    /// </summary>
    public IUiMediator ClientMediator { get; } = Substitute.For<IUiMediator>();

    /// <summary>
    /// Gets the substitute registered as <see cref="IActiveNotificationElementPolicy"/>.
    /// </summary>
    public IActiveNotificationElementPolicy ActiveNotificationElementPolicy { get; } = Substitute.For<IActiveNotificationElementPolicy>();

    /// <summary>
    /// Gets the <see cref="IJSRuntime"/> substitute; registered only when <see cref="UseSimpleJsMock"/> is set.
    /// </summary>
    public IJSRuntime JSRuntime { get; } = Substitute.For<IJSRuntime>();

    /// <summary>
    /// Gets the substitute registered as <see cref="IMessageBannerService"/>.
    /// </summary>
    public IMessageBannerService MessageBanner { get; } = Substitute.For<IMessageBannerService>();

    /// <summary>
    /// Gets the substitute registered as <see cref="IControlPanelRegistryFactory"/>.
    /// </summary>
    public IControlPanelRegistryFactory ControlPanelRegistryFactory { get; } = Substitute.For<IControlPanelRegistryFactory>();

    /// <summary>
    /// Gets the substitute registered as <see cref="INavTileRegistryFactory"/>.
    /// </summary>
    public INavTileRegistryFactory NavTileRegistryFactory { get; } = Substitute.For<INavTileRegistryFactory>();

    /// <summary>
    /// Gets the substitute registered as <see cref="INotificationElementRegistryFactory"/>.
    /// </summary>
    public INotificationElementRegistryFactory NotificationElementRegistryFactory { get; } = Substitute.For<INotificationElementRegistryFactory>();

    /// <summary>
    /// Gets or sets whether a <see cref="NavigationManager"/> is registered that records navigation instead of performing it;
    /// navigating to <c>/</c> resets the URI to the base URI. Defaults to true.
    /// </summary>
    public bool UseNavigationManager { get; set; } = true;

    internal NavigationManager NavigationManagerMock { get; } = new MockNavigationManager();

    private sealed class MockNavigationManager
        : NavigationManager
    {
        public bool WasNavigateInvoked { get; private set; }

        public MockNavigationManager() => Initialize("http://localhost:2112/", "http://localhost:2112/test");

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            if (uri == "/")
            {
                Uri = BaseUri;
                NotifyLocationChanged(false);
            }

            WasNavigateInvoked = true;
        }

        protected override void SetNavigationLockState(bool value) { }
    }
}
