using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Backend.Modules;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public sealed class IServiceCollectionExtensionsTests
{
    public sealed class AddMvcBuilder
    {
        [Fact]
        public void Should_setup_default_mvc_builder()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var serviceProvider = services
                .AddMvcBuilder()
                .BuildServiceProvider();

            // Assert
            var mvcBuilder = serviceProvider.GetRequiredService<IMvcBuilder>();

            mvcBuilder.Should().NotBeNull();
            mvcBuilder.PartManager.Should().NotBeNull();
        }
    }

    public sealed class AddWorkspaceService
    {
        [Fact]
        public void Should_setup_workspace_service_with_default_root()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var serviceProvider = services
                .AddWorkspaceService<LocalTestBackendModule>()
                .BuildServiceProvider();

            // Assert
            var workspaceProvider = serviceProvider.GetRequiredService<IWorkspaceProvider<LocalTestBackendModule>>();

            workspaceProvider.Home.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Should_setup_workspace_service_with_setup()
        {
            // Arrange
            var services = new ServiceCollection();
            const string Home = "/some/path/home";
            const string Cache = "/some/path/cache";

            // Act
            var serviceProvider = services
                .AddWorkspaceService<LocalTestBackendModule>(setup =>
                {
                    setup.Home.Returns(Home);
                    setup.Cache.Returns(Cache);
                })
                .BuildServiceProvider();

            // Assert
            var workspaceProvider = serviceProvider.GetRequiredService<IWorkspaceProvider<LocalTestBackendModule>>();

            workspaceProvider.Home.Should().Be(Home);
            workspaceProvider.Cache.Should().Be(Cache);
        }
    }

    public sealed class LocalTestBackendModule : BackendModule;
}
