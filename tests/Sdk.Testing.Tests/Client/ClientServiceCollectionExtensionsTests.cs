using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Infrastructure;
using Sdk.Client.Modules;
using Sdk.Client.Modules.Localization;
using Sdk.Client.Services;
using Sdk.Instance;
using Sdk.Messaging;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Testing.Tests.Client;

public class ClientServiceCollectionExtensionsTests
{
    public class AddClientServices
    {
        [Fact]
        public void Should_add_client_services()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var serviceProvider = services
                .AddClientServices(null)
                .BuildServiceProvider();

            // Assert
            serviceProvider.GetRequiredService<IConnectionService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IControlPanelService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IInstanceInformationProvider>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IUiMediator>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IClientModuleService>().Should().NotBeNull();
        }

        [Fact]
        public void Should_setup_local_instance_information()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var serviceProvider = services
                .AddClientServices(null)
                .BuildServiceProvider();

            // Assert
            var informationProvider = serviceProvider.GetRequiredService<IInstanceInformationProvider>();

            informationProvider.Local.Should().NotBeNull();
        }
    }

    public class AddHttpClient
    {
        [Fact]
        public async Task Should_setup_fake_client()
        {
            // Arrange
            var services = new ServiceCollection();
            var response = new ErrorInfo(101, "Test");

            // Act
            var serviceProvider = services
                .AddHttpClient(response, null)
                .BuildServiceProvider();

            // Assert
            var client = serviceProvider.GetRequiredService<HttpClient>();
            var result = await client.GetFromJsonAsync<ErrorInfo>("url");
            result.Should().Be(response);

        }
    }

    public class AddLocalization
    {
        [Fact]
        public void Should_add_client_module_localizer()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var serviceProvider = services
                .AddLocalization<TestClientModule>()
                .BuildServiceProvider();

            // Assert
            var moduleLocalizer = serviceProvider.GetRequiredService<IClientModuleLocalizer<TestClientModule>>();
            moduleLocalizer.GetTitle().Should().NotBeNullOrEmpty();
            moduleLocalizer.GetDescription().Should().NotBeNullOrEmpty();
        }

        public class TestClientModule : ClientModule;
    }
}
