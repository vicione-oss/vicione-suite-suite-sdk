using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels;
using Sdk.Client.ControlPanels.Builders;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels;

public sealed class SharedControlPanelStateTests
{
    internal sealed class TestControlPanelState : ControlPanelState;

    internal sealed class ParentControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class TabControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class OtherControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class ParentControlPanelDescriptor : IControlPanelDescriptor<ParentControlPanel>
    {
        public string Title => "Parent";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    internal sealed class TabControlPanelDescriptor : IControlPanelDescriptor<TabControlPanel>
    {
        public string Title => "Tab";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    internal sealed class OtherControlPanelDescriptor : IControlPanelDescriptor<OtherControlPanel>
    {
        public string Title => "Other";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    private sealed class OtherSharedState : ISharedControlPanelState<TestControlPanelState>;

    private sealed class OtherControlPanelBuilder : IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState>
    {
        public IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> WithAutoDiscovery<TDescriptor>()
            where TDescriptor : class, IControlPanelDescriptor<ParentControlPanel>
            => this;

        public IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> WithSaveHandler<TSaveHandler>()
            where TSaveHandler : class, IControlPanelSaveHandler<TestControlPanelState>
            => this;

        public IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> WithCancelHandler<TCancelHandler>()
            where TCancelHandler : class, IControlPanelCancelHandler<TestControlPanelState>
            => this;

        public IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> WithResetHandler<TResetHandler>()
            where TResetHandler : class, IControlPanelResetHandler<TestControlPanelState>
            => this;
    }

    private static readonly Type s_parentKey = typeof(ControlPanelServiceKey<ParentControlPanel>);
    private static readonly Type s_tabKey = typeof(ControlPanelServiceKey<TabControlPanel>);
    private static readonly Type s_otherKey = typeof(ControlPanelServiceKey<OtherControlPanel>);

    private readonly ServiceCollection _services = new();

    private ControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> ParentBuilder => new(_services);

    private ControlPanelBuilder<TestClientModule, TabControlPanel, TestControlPanelState> TabBuilder => new(_services);

    [Fact]
    public void Should_give_a_control_panel_the_state_shared_with_it()
    {
        // Arrange
        ParentBuilder.WithAutoDiscovery<ParentControlPanelDescriptor>().WithSharedState(out var sharedState);
        TabBuilder.WithAutoDiscovery<TabControlPanelDescriptor>().WithSharedState(sharedState);

        using var serviceProvider = BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        // Act
        var parentState = GetState(scope, s_parentKey);
        var tabState = GetState(scope, s_tabKey);

        // Assert
        tabState.Should().BeSameAs(parentState);
    }

    [Fact]
    public void Should_give_a_control_panel_the_state_shared_with_it_before_its_auto_discovery()
    {
        // Arrange
        ParentBuilder.WithSharedState(out var sharedState).WithAutoDiscovery<ParentControlPanelDescriptor>();
        TabBuilder.WithSharedState(sharedState).WithAutoDiscovery<TabControlPanelDescriptor>();

        using var serviceProvider = BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        // Act
        var parentState = GetState(scope, s_parentKey);
        var tabState = GetState(scope, s_tabKey);

        // Assert
        tabState.Should().BeSameAs(parentState);
        _services.Should().ContainSingle(d => d.ServiceType == typeof(TestControlPanelState) && Equals(d.ServiceKey, s_parentKey));
        _services.Should().ContainSingle(d => d.ServiceType == typeof(TestControlPanelState) && Equals(d.ServiceKey, s_tabKey));
    }

    [Fact]
    public void Should_share_the_state_of_a_control_panel_without_auto_discovery()
    {
        // Arrange
        ParentBuilder.WithSharedState(out var sharedState);
        TabBuilder.WithSharedState(sharedState);

        using var serviceProvider = BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        // Act
        var parentState = GetState(scope, s_parentKey);
        var tabState = GetState(scope, s_tabKey);

        // Assert
        tabState.Should().BeSameAs(parentState);
    }

    [Fact]
    public void Should_throw_when_a_control_panel_is_given_the_state_it_shares_itself()
    {
        // Arrange
        var parentBuilder = ParentBuilder.WithAutoDiscovery<ParentControlPanelDescriptor>().WithSharedState(out var sharedState);

        // Act
        var act = () => parentBuilder.WithSharedState(sharedState);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"*'{typeof(ParentControlPanel)}'*");
        _services.Should().ContainSingle(d => d.ServiceType == typeof(TestControlPanelState) && Equals(d.ServiceKey, s_parentKey));
    }

    [Fact]
    public void Should_keep_the_state_of_a_control_panel_not_sharing_it()
    {
        // Arrange
        ParentBuilder.WithAutoDiscovery<ParentControlPanelDescriptor>().WithSharedState(out _);
        new ControlPanelBuilder<TestClientModule, OtherControlPanel, TestControlPanelState>(_services)
            .WithAutoDiscovery<OtherControlPanelDescriptor>();

        using var serviceProvider = BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        // Act
        var parentState = GetState(scope, s_parentKey);
        var otherState = GetState(scope, s_otherKey);

        // Assert
        otherState.Should().NotBeSameAs(parentState);
    }

    [Fact]
    public void Should_share_one_state_per_scope()
    {
        // Arrange
        ParentBuilder.WithAutoDiscovery<ParentControlPanelDescriptor>().WithSharedState(out var sharedState);
        TabBuilder.WithAutoDiscovery<TabControlPanelDescriptor>().WithSharedState(sharedState);

        using var serviceProvider = BuildServiceProvider();
        using var firstScope = serviceProvider.CreateScope();
        using var secondScope = serviceProvider.CreateScope();

        // Act
        var firstTabState = GetState(firstScope, s_tabKey);
        var secondTabState = GetState(secondScope, s_tabKey);

        // Assert
        secondTabState.Should().NotBeSameAs(firstTabState);
        secondTabState.Should().BeSameAs(GetState(secondScope, s_parentKey));
    }

    [Fact]
    public void Should_not_support_a_shared_state_on_a_builder_other_than_the_sdks()
    {
        // Arrange
        IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> builder = new OtherControlPanelBuilder();

        // Act
        var act = () => builder.WithSharedState(out _);

        // Assert
        act.Should().Throw<NotSupportedException>();
    }

    private ServiceProvider BuildServiceProvider()
        => _services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    [Fact]
    public void Should_not_support_a_shared_state_not_created_by_the_sdk()
    {
        // Arrange
        var tabBuilder = TabBuilder.WithAutoDiscovery<TabControlPanelDescriptor>();

        // Act
        var act = () => tabBuilder.WithSharedState(new OtherSharedState());

        // Assert
        act.Should().Throw<NotSupportedException>();
        _services.Should().ContainSingle(d => d.ServiceType == typeof(TestControlPanelState) && Equals(d.ServiceKey, s_tabKey));
    }

    private static TestControlPanelState GetState(IServiceScope scope, Type key)
        => scope.ServiceProvider.GetRequiredKeyedService<TestControlPanelState>(key);
}
