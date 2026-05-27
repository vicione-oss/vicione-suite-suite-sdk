using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Authorization;
using Sdk.Client.ControlPanels.Attributes;
using Sdk.Client.ControlPanels.Builders;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Models;
using Sdk.Client.ControlPanels.Services;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels;

public sealed class ControlPanelBuilderTests
{
    internal sealed class TestControlPanelState : ControlPanelState;

    [ControlPanelCategory<TestControlPanelCategoryDescriptor>]
    [ControlPanelGroup<TestControlPanelGroupDescriptor>]
    [ModuleAuthorize<TestClientModule>(AccessLevel.Full)]
    internal sealed class TestControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class TestControlPanelDescriptor : IControlPanelDescriptor<TestControlPanel>
    {
        public string Title => "Test control panel";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<IControlPanelNetworkCategoryDescriptor>]
    internal sealed class TestNetworkControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class TestNetworkControlPanelDescriptor : IControlPanelDescriptor<TestNetworkControlPanel>
    {
        public string Title => "Test network control panel";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    internal sealed class TestControlPanelSaveHandler : IControlPanelSaveHandler<TestControlPanelState>
    {
        public Task<ISaveResult> Save(TestControlPanelState state, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    internal sealed class TestControlPanelCancelHandler : IControlPanelCancelHandler<TestControlPanelState>
    {
        public Task Cancel(TestControlPanelState state, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    internal sealed class TestControlPanelResetHandler : IControlPanelResetHandler<TestControlPanelState>
    {
        public Task Reset(TestControlPanelState state, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    internal sealed class TestControlPanelCategoryDescriptor : IControlPanelCategoryDescriptor
    {
        public string Title => "Test category";
    }

    internal sealed class TestControlPanelNetworkCategoryDescriptor : IControlPanelNetworkCategoryDescriptor
    {
        public string Title => "Network category";
    }

    internal sealed class TestControlPanelGroupDescriptor : IControlPanelGroupDescriptor
    {
        public int Position => 0;
    }

    [Fact]
    public void Should_configure_auto_discovery()
    {
        // Arrange
        var services = new ServiceCollection();
        var controlPanelBuilder = new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services);

        controlPanelBuilder.WithAutoDiscovery<TestControlPanelDescriptor>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var controlPanelInfo = serviceProvider.GetKeyedService<ControlPanelInfo>(typeof(TestClientModule));

        // Assert
        controlPanelInfo.Should().NotBeNull();
        controlPanelInfo!.ComponentType.Should().Be<TestControlPanel>();
        controlPanelInfo.StateType.Should().Be<TestControlPanelState>();
        controlPanelInfo.DescriptorType.Should().Be<TestControlPanelDescriptor>();
        controlPanelInfo.CategoryDescriptorType.Should().Be<TestControlPanelCategoryDescriptor>();
        controlPanelInfo.GroupDescriptorType.Should().Be<TestControlPanelGroupDescriptor>();
        controlPanelInfo.ModuleAuthorizeAttribute.Should().NotBeNull();

        serviceProvider.GetKeyedService<IControlPanelDescriptor<TestControlPanel>>(controlPanelInfo.KeyedServiceKey)
            .Should().BeOfType<TestControlPanelDescriptor>();

        serviceProvider.GetKeyedServices<TestControlPanelState>(controlPanelInfo.KeyedServiceKey).Should().NotBeNull();

        serviceProvider.GetService<TestControlPanelCategoryDescriptor>().Should().NotBeNull();
        serviceProvider.GetService<TestControlPanelGroupDescriptor>().Should().NotBeNull();
    }

    [Fact]
    public void Should_configure_network_category_descriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IControlPanelNetworkCategoryDescriptor, TestControlPanelNetworkCategoryDescriptor>();
        var controlPanelBuilder = new ControlPanelBuilder<TestClientModule, TestNetworkControlPanel, TestControlPanelState>(services);

        controlPanelBuilder.WithAutoDiscovery<TestNetworkControlPanelDescriptor>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var controlPanelInfo = serviceProvider.GetKeyedService<ControlPanelInfo>(typeof(TestClientModule));

        // Assert
        controlPanelInfo.Should().NotBeNull();
        controlPanelInfo!.ComponentType.Should().Be<TestNetworkControlPanel>();
        controlPanelInfo.StateType.Should().Be<TestControlPanelState>();
        controlPanelInfo.DescriptorType.Should().Be<TestNetworkControlPanelDescriptor>();
        controlPanelInfo.CategoryDescriptorType.Should().Be<IControlPanelNetworkCategoryDescriptor>();

        serviceProvider.GetKeyedService<IControlPanelDescriptor<TestNetworkControlPanel>>(controlPanelInfo.KeyedServiceKey)
            .Should().BeOfType<TestNetworkControlPanelDescriptor>();

        serviceProvider.GetKeyedServices<TestControlPanelState>(controlPanelInfo.KeyedServiceKey).Should().NotBeNull();

        serviceProvider.GetService<IControlPanelNetworkCategoryDescriptor>().Should().NotBeNull();
    }

    [Fact]
    public void Should_configure_save_handler()
    {
        // Arrange
        var services = new ServiceCollection();
        var controlPanelBuilder = new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services);

        controlPanelBuilder.WithSaveHandler<TestControlPanelSaveHandler>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var saveHandler = serviceProvider.GetService<IControlPanelSaveHandler<TestControlPanelState>>();

        // Assert
        saveHandler.Should().NotBeNull();
    }

    [Fact]
    public void Should_configure_cancel_handler()
    {
        // Arrange
        var services = new ServiceCollection();
        var controlPanelBuilder = new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services);

        controlPanelBuilder.WithCancelHandler<TestControlPanelCancelHandler>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var cancelHandler = serviceProvider.GetService<IControlPanelCancelHandler<TestControlPanelState>>();

        // Assert
        cancelHandler.Should().NotBeNull();
    }

    [Fact]
    public void Should_configure_reset_handler()
    {
        // Arrange
        var services = new ServiceCollection();
        var controlPanelBuilder = new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services);

        controlPanelBuilder.WithResetHandler<TestControlPanelResetHandler>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var saveHandler = serviceProvider.GetService<IControlPanelResetHandler<TestControlPanelState>>();

        // Assert
        saveHandler.Should().NotBeNull();
    }
}
