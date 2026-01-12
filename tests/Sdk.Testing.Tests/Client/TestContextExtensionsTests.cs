using System.Globalization;
using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sdk.Client.Infrastructure;
using Sdk.Client.Services;
using Sdk.Instance;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Testing.Tests.Client;

public sealed class TestContextExtensionsTests
{
    public sealed class SetupSuiteServices
    {
        [Fact]
        public void Should_setup_ui_culture()
        {
            // Arrange
            using var context = new TestContext();
            var cultureInfo = new CultureInfo("de-DE");

            // Act
            context.SetupSuiteServices(null, "de-DE");

            // Assert
            CultureInfo.DefaultThreadCurrentCulture.Should().BeEquivalentTo(cultureInfo);
            CultureInfo.DefaultThreadCurrentUICulture.Should().BeEquivalentTo(cultureInfo);
        }

        [Fact]
        public void Should_add_localization()
        {
            // Arrange
            using var context = new TestContext();

            // Act
            context.SetupSuiteServices();

            // Assert
            context.Services.GetRequiredService<IStringLocalizerFactory>().Should().NotBeNull();
        }

        [Fact]
        public void Should_add_logging()
        {
            // Arrange
            using var context = new TestContext();

            // Act
            context.SetupSuiteServices();

            // Assert
            context.Services.GetRequiredService<ILoggerFactory>().Should().NotBeNull();
        }

        [Fact]
        public void Should_setup_suite_client_services()
        {
            // Arrange
            using var context = new TestContext();

            // Act
            context.SetupSuiteServices();

            // Assert
            context.Services.GetRequiredService<IConnectionService>().Should().NotBeNull();
            context.Services.GetRequiredService<IInstanceInformationProvider>().Should().NotBeNull();
            context.Services.GetRequiredService<IUiMediator>().Should().NotBeNull();
#pragma warning disable CS0618 // Type or member is obsolete
            context.Services.GetRequiredService<IClientModuleService>().Should().NotBeNull();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public async Task Should_setup_optional_fake_authentication()
        {
            // Arrange
            using var context = new TestContext();

            // Act
            context.SetupSuiteServices(setup => setup.FakeAuthenticationStateProvider = true);

            // Assert
            var authProvider = context.Services.GetRequiredService<AuthenticationStateProvider>();
            var state = await authProvider.GetAuthenticationStateAsync();
            state.User.Should().NotBeNull();
        }
    }
}
