using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Backend.Modules;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public class ServiceCollectionExtensionsTests
{
    public class AddMvcBuilder
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

    public class AddWorkspaceService
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
            var home = "/some/path/home";
            var cache = "/some/path/cache";

            // Act
            var serviceProvider = services
                .AddWorkspaceService<LocalTestBackendModule>(setup =>
                {
                    setup.Home.Returns(home);
                    setup.Cache.Returns(cache);
                })
                .BuildServiceProvider();

            // Assert
            var workspaceProvider = serviceProvider.GetRequiredService<IWorkspaceProvider<LocalTestBackendModule>>();

            workspaceProvider.Home.Should().Be(home);
            workspaceProvider.Cache.Should().Be(cache);
        }
    }

    public class LocalTestBackendModule : BackendModule;
}
