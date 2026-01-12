using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Extensions;
using Sdk.Client.ControlPanels.Services;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels.Extensions;

public class IServiceCollectionExtensionsTests
{
    internal sealed class TestControlPanel : ControlPanelBase<ControlPanelState>;

    public sealed class AddControlPanel : IServiceCollectionExtensionsTests
    {
        [Fact]
        public void Should_add_core_services_and_return_builder()
        {
            // Arrange
            var services = new ServiceCollection();

            var builder = services.AddControlPanel<TestClientModule, TestControlPanel, ControlPanelState>();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var controlPanelRegistry = serviceProvider.GetService<IControlPanelRegistry<TestClientModule>>();
            var controlPanelPageRegistry = serviceProvider.GetService<IControlPanelPageRegistry>();

            // Assert
            controlPanelRegistry.Should().NotBeNull();
            controlPanelPageRegistry.Should().NotBeNull();
            builder.Should().NotBeNull();
        }
    }
}
