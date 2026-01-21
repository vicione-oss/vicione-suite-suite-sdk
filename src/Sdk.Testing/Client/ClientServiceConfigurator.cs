using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using NSubstitute;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Infrastructure;
using Sdk.Client.NavTiles.Services;
using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Services;
using Sdk.Instance;

namespace Sdk.Testing.Client;

/// <summary>
/// A helper class for configuring mocked services for client-side testing.
/// </summary>
public sealed class ClientServiceConfigurator(IServiceCollection services)
{
    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// Gets a mocked <see cref="IStringLocalizerFactory"/> for testing localization.
    /// </summary>
    public IStringLocalizerFactory StringLocalizerFactory { get; } = Substitute.For<IStringLocalizerFactory>();

    /// <summary>
    /// Gets or sets a value indicating whether a simple mock of <see cref="IJSRuntime"/> should be used.
    /// </summary>
    public bool UseSimpleJsMock { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a fake <see cref="AuthenticationStateProvider"/> should be registered. Defaults to true.
    /// </summary>
    public bool FakeAuthenticationStateProvider { get; set; } = true;

    /// <summary>
    /// Gets a mocked <see cref="IConnectionService"/> for testing connection-related logic.
    /// </summary>
    public IConnectionService ConnectionService { get; } = Substitute.For<IConnectionService>();

    /// <summary>
    /// Gets a mocked <see cref="ILayoutService"/> for testing layout modifications.
    /// </summary>
    public ILayoutService Layout { get; } = Substitute.For<ILayoutService>();

    /// <summary>
    /// Gets a mocked <see cref="IJsInterop"/> for testing JavaScript interoperability.
    /// </summary>
    public IJsInterop JsInterop { get; } = Substitute.For<IJsInterop>();

    /// <summary>
    /// Gets a mocked <see cref="IInstanceInformationProvider"/> for providing fake instance data.
    /// </summary>
    public IInstanceInformationProvider InstanceInformationProvider { get; } = Substitute.For<IInstanceInformationProvider>();

    /// <summary>
    /// Gets a mocked <see cref="IUiMediator"/> for testing UI-to-backend communication.
    /// </summary>
    public IUiMediator ClientMediator { get; } = Substitute.For<IUiMediator>();

    /// <summary>
    /// Gets a mocked <see cref="IActiveNotificationElementPolicy"/> for testing notification policies.
    /// </summary>
    public IActiveNotificationElementPolicy ActiveNotificationElementPolicy { get; } = Substitute.For<IActiveNotificationElementPolicy>();

    /// <summary>
    /// Gets a mocked <see cref="IJSRuntime"/> for testing JavaScript interop.
    /// </summary>
    public IJSRuntime JSRuntime { get; } = Substitute.For<IJSRuntime>();

    /// <summary>
    /// Gets a mocked <see cref="IMessageBannerService"/> for testing message banners.
    /// </summary>
    public IMessageBannerService MessageBanner { get; } = Substitute.For<IMessageBannerService>();

    /// <summary>
    /// Gets a mocked <see cref="IControlPanelRegistryFactory"/> for testing control panel registration.
    /// </summary>
    public IControlPanelRegistryFactory ControlPanelRegistryFactory { get; } = Substitute.For<IControlPanelRegistryFactory>();

    /// <summary>
    /// Gets a mocked <see cref="INavTileRegistryFactory"/> for testing navigation tile registration.
    /// </summary>
    public INavTileRegistryFactory NavTileRegistryFactory { get; } = Substitute.For<INavTileRegistryFactory>();

    /// <summary>
    /// Gets a mocked <see cref="INotificationElementRegistryFactory"/> for testing notification element registration.
    /// </summary>
    public INotificationElementRegistryFactory NotificationElementRegistryFactory { get; } = Substitute.For<INotificationElementRegistryFactory>();

    /// <summary>
    /// Gets or sets a value indicating whether a mocked <see cref="NavigationManager"/> should be used. Defaults to true.
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
