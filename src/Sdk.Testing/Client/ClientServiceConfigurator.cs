using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using NSubstitute;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Infrastructure;
using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Services;
using Sdk.Instance;

namespace Sdk.Testing.Client;

public sealed class ClientServiceConfigurator(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
    public IStringLocalizerFactory StringLocalizerFactory { get; } = Substitute.For<IStringLocalizerFactory>();
    public bool UseSimpleJsMock { get; set; }
    public bool FakeAuthenticationStateProvider { get; set; } = true;

    // used in dx component base since 21.1.3
    public IConnectionService ConnectionService { get; } = Substitute.For<IConnectionService>();
    public ILayoutService Layout { get; } = Substitute.For<ILayoutService>();
    public IJsInterop JsInterop { get; } = Substitute.For<IJsInterop>();
    public IInstanceInformationProvider InstanceInformationProvider { get; } = Substitute.For<IInstanceInformationProvider>();
    public IUiMediator ClientMediator { get; } = Substitute.For<IUiMediator>();
    public IClientModuleService ClientModuleService { get; } = Substitute.For<IClientModuleService>();
    public IControlPanelService ControlPanelService { get; } = Substitute.For<IControlPanelService>();
    public IActiveNotificationElementPolicy ActiveNotificationElementPolicy { get; } = Substitute.For<IActiveNotificationElementPolicy>();
    public IJSRuntime JSRuntime { get; } = Substitute.For<IJSRuntime>();
    public IMessageBannerService MessageBanner { get; } = Substitute.For<IMessageBannerService>();

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
