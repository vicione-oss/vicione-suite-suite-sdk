using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Authorization;
using Sdk.Client.ControlPanels;
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

    internal sealed class TestDependency;

    internal sealed class TestControlPanelSaveHandlerWithDependency(TestDependency dependency) : IControlPanelSaveHandler<TestControlPanelState>
    {
        public TestDependency Dependency => dependency;

        public Task<ISaveResult> Save(TestControlPanelState state, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    internal sealed class TestNetworkControlPanelSaveHandler : IControlPanelSaveHandler<TestControlPanelState>
    {
        public Task<ISaveResult> Save(TestControlPanelState state, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    internal sealed class TestNetworkControlPanelCancelHandler : IControlPanelCancelHandler<TestControlPanelState>
    {
        public Task Cancel(TestControlPanelState state, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    internal sealed class TestNetworkControlPanelResetHandler : IControlPanelResetHandler<TestControlPanelState>
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

        serviceProvider.GetKeyedService<IControlPanelDescriptor<TestControlPanel>>(controlPanelInfo.ServiceKey)
            .Should().BeOfType<TestControlPanelDescriptor>();

        serviceProvider.GetKeyedServices<TestControlPanelState>(controlPanelInfo.ServiceKey).Should().NotBeNull();

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

        serviceProvider.GetKeyedService<IControlPanelDescriptor<TestNetworkControlPanel>>(controlPanelInfo.ServiceKey)
            .Should().BeOfType<TestNetworkControlPanelDescriptor>();

        serviceProvider.GetKeyedServices<TestControlPanelState>(controlPanelInfo.ServiceKey).Should().NotBeNull();

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
        var saveHandler = serviceProvider.GetKeyedService<IControlPanelSaveHandler<TestControlPanelState>>(
            typeof(ControlPanelServiceKey<TestControlPanel>));

        // Assert
        saveHandler.Should().BeOfType<TestControlPanelSaveHandler>();
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
        var cancelHandler = serviceProvider.GetKeyedService<IControlPanelCancelHandler<TestControlPanelState>>(
            typeof(ControlPanelServiceKey<TestControlPanel>));

        // Assert
        cancelHandler.Should().BeOfType<TestControlPanelCancelHandler>();
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
        var resetHandler = serviceProvider.GetKeyedService<IControlPanelResetHandler<TestControlPanelState>>(
            typeof(ControlPanelServiceKey<TestControlPanel>));

        // Assert
        resetHandler.Should().BeOfType<TestControlPanelResetHandler>();
    }

    [Fact]
    public void Should_configure_handlers_per_control_panel_when_control_panels_share_a_state()
    {
        // Arrange
        var services = new ServiceCollection();

        new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services)
            .WithSaveHandler<TestControlPanelSaveHandler>()
            .WithCancelHandler<TestControlPanelCancelHandler>()
            .WithResetHandler<TestControlPanelResetHandler>();

        new ControlPanelBuilder<TestClientModule, TestNetworkControlPanel, TestControlPanelState>(services)
            .WithSaveHandler<TestNetworkControlPanelSaveHandler>()
            .WithCancelHandler<TestNetworkControlPanelCancelHandler>()
            .WithResetHandler<TestNetworkControlPanelResetHandler>();

        var serviceProvider = services.BuildServiceProvider();

        var key = typeof(ControlPanelServiceKey<TestControlPanel>);
        var networkKey = typeof(ControlPanelServiceKey<TestNetworkControlPanel>);

        // Act & Assert
        serviceProvider.GetKeyedService<IControlPanelSaveHandler<TestControlPanelState>>(key)
            .Should().BeOfType<TestControlPanelSaveHandler>();
        serviceProvider.GetKeyedService<IControlPanelCancelHandler<TestControlPanelState>>(key)
            .Should().BeOfType<TestControlPanelCancelHandler>();
        serviceProvider.GetKeyedService<IControlPanelResetHandler<TestControlPanelState>>(key)
            .Should().BeOfType<TestControlPanelResetHandler>();

        serviceProvider.GetKeyedService<IControlPanelSaveHandler<TestControlPanelState>>(networkKey)
            .Should().BeOfType<TestNetworkControlPanelSaveHandler>();
        serviceProvider.GetKeyedService<IControlPanelCancelHandler<TestControlPanelState>>(networkKey)
            .Should().BeOfType<TestNetworkControlPanelCancelHandler>();
        serviceProvider.GetKeyedService<IControlPanelResetHandler<TestControlPanelState>>(networkKey)
            .Should().BeOfType<TestNetworkControlPanelResetHandler>();
    }

    [Fact]
    public void Should_inject_services_registered_without_key_into_a_handler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<TestDependency>();

        new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services)
            .WithSaveHandler<TestControlPanelSaveHandlerWithDependency>();

        var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using var scope = serviceProvider.CreateScope();

        // Act
        var saveHandler = scope.ServiceProvider.GetKeyedService<IControlPanelSaveHandler<TestControlPanelState>>(
            typeof(ControlPanelServiceKey<TestControlPanel>));

        // Assert
        saveHandler.Should().BeOfType<TestControlPanelSaveHandlerWithDependency>()
            .Which.Dependency.Should().BeSameAs(scope.ServiceProvider.GetRequiredService<TestDependency>());
    }

    [Fact]
    public void Should_not_configure_handlers_for_other_control_panels_of_the_same_state()
    {
        // Arrange
        var services = new ServiceCollection();

        new ControlPanelBuilder<TestClientModule, TestControlPanel, TestControlPanelState>(services)
            .WithSaveHandler<TestControlPanelSaveHandler>()
            .WithCancelHandler<TestControlPanelCancelHandler>()
            .WithResetHandler<TestControlPanelResetHandler>();

        var serviceProvider = services.BuildServiceProvider();

        var otherKey = typeof(ControlPanelServiceKey<TestNetworkControlPanel>);

        // Act & Assert
        serviceProvider.GetKeyedService<IControlPanelSaveHandler<TestControlPanelState>>(otherKey).Should().BeNull();
        serviceProvider.GetKeyedService<IControlPanelCancelHandler<TestControlPanelState>>(otherKey).Should().BeNull();
        serviceProvider.GetKeyedService<IControlPanelResetHandler<TestControlPanelState>>(otherKey).Should().BeNull();

        serviceProvider.GetService<IControlPanelSaveHandler<TestControlPanelState>>().Should().BeNull();
        serviceProvider.GetService<IControlPanelCancelHandler<TestControlPanelState>>().Should().BeNull();
        serviceProvider.GetService<IControlPanelResetHandler<TestControlPanelState>>().Should().BeNull();
    }
}
