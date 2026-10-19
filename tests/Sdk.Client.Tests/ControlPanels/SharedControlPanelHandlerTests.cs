using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels;
using Sdk.Client.ControlPanels.Builders;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Models;
using Sdk.Client.ControlPanels.Services;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels;

public sealed class SharedControlPanelHandlerTests : IAsyncDisposable
{
    internal sealed class TestControlPanelState : ControlPanelState;

    internal sealed class ParentControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class TabControlPanel : ControlPanelBase<TestControlPanelState>;

    /// <summary>
    /// Records the invocations of the handlers below and lets a test hold them until it completes <see cref="Release"/>.
    /// </summary>
    internal sealed class InvocationLog
    {
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool Hold { get; set; }

        public List<object> Handlers { get; } = [];

        public List<object> CanceledHandlers { get; } = [];

        public async Task Record(object handler, CancellationToken cancellationToken)
        {
            Handlers.Add(handler);

            if (!Hold)
                return;

            try
            {
                await Release.Task.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                CanceledHandlers.Add(handler);

                throw;
            }
        }
    }

    internal sealed class FirstSaveHandler(InvocationLog invocations) : IControlPanelSaveHandler<TestControlPanelState>
    {
        public async Task<ISaveResult> Save(TestControlPanelState state, CancellationToken cancellationToken)
        {
            await invocations.Record(this, cancellationToken);

            return new SaveSuccessResult();
        }
    }

    internal sealed class SecondSaveHandler(InvocationLog invocations) : IControlPanelSaveHandler<TestControlPanelState>
    {
        public async Task<ISaveResult> Save(TestControlPanelState state, CancellationToken cancellationToken)
        {
            await invocations.Record(this, cancellationToken);

            return new SaveSuccessResult();
        }
    }

    internal sealed class TestCancelHandler(InvocationLog invocations) : IControlPanelCancelHandler<TestControlPanelState>
    {
        public Task Cancel(TestControlPanelState state, CancellationToken cancellationToken) => invocations.Record(this, cancellationToken);
    }

    internal sealed class TestResetHandler(InvocationLog invocations) : IControlPanelResetHandler<TestControlPanelState>
    {
        public Task Reset(TestControlPanelState state, CancellationToken cancellationToken) => invocations.Record(this, cancellationToken);
    }

    internal sealed class TestCancelAndResetHandler(InvocationLog invocations)
        : IControlPanelCancelHandler<TestControlPanelState>, IControlPanelResetHandler<TestControlPanelState>
    {
        public Task Cancel(TestControlPanelState state, CancellationToken cancellationToken) => invocations.Record(this, cancellationToken);

        public Task Reset(TestControlPanelState state, CancellationToken cancellationToken) => invocations.Record(this, cancellationToken);
    }

    private sealed class OtherSharedHandlers : ISharedControlPanelHandlers<TestControlPanelState>;

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

    private readonly ServiceCollection _services = new();
    private ServiceProvider? _serviceProvider;
    private AsyncServiceScope _scope;

    private InvocationLog Invocations => _scope.ServiceProvider.GetRequiredService<InvocationLog>();

    private IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> ParentBuilder
        => new ControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState>(_services);

    private IControlPanelBuilder<TestClientModule, TabControlPanel, TestControlPanelState> TabBuilder
        => new ControlPanelBuilder<TestClientModule, TabControlPanel, TestControlPanelState>(_services);

    public SharedControlPanelHandlerTests()
        => _services.AddScoped<InvocationLog>();

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider is null)
            return;

        await _scope.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }

    [Fact]
    public void Should_configure_the_shared_handlers_for_the_owner_and_each_control_panel_passed_them()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>().WithResetHandler<TestResetHandler>());

        // Act
        var parentSaveHandler = GetSaveHandler(s_parentKey);
        var tabSaveHandler = GetSaveHandler(s_tabKey);
        var tabResetHandler = GetResetHandler(s_tabKey);

        // Assert
        var parentSharedHandler = parentSaveHandler.Should().BeAssignableTo<ISharedControlPanelHandler>().Subject;
        parentSharedHandler.HandlerType.Should().Be<FirstSaveHandler>();
        parentSharedHandler.OwnerServiceKey.Should().Be(s_parentKey);

        var tabSharedHandler = tabSaveHandler.Should().BeAssignableTo<ISharedControlPanelHandler>().Subject;
        tabSharedHandler.HandlerType.Should().Be<FirstSaveHandler>();
        tabSharedHandler.OwnerServiceKey.Should().Be(s_parentKey);

        tabResetHandler.Should().BeAssignableTo<ISharedControlPanelHandler>()
            .Which.HandlerType.Should().Be<TestResetHandler>();
    }

    [Fact]
    public void Should_configure_only_the_handlers_shared()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());

        // Act
        var tabCancelHandler = _scope.ServiceProvider.GetKeyedService<IControlPanelCancelHandler<TestControlPanelState>>(s_tabKey);
        var tabResetHandler = _scope.ServiceProvider.GetKeyedService<IControlPanelResetHandler<TestControlPanelState>>(s_tabKey);

        // Assert
        tabCancelHandler.Should().BeNull();
        tabResetHandler.Should().BeNull();
    }

    [Fact]
    public void Should_throw_when_the_owner_has_a_handler_of_its_own_for_an_operation_it_shares()
    {
        // Arrange
        var parentBuilder = ParentBuilder.WithSaveHandler<SecondSaveHandler>();

        // Act
        var act = () => parentBuilder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out _);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*'{typeof(ParentControlPanel)}'*save handler*'{typeof(FirstSaveHandler)}'*");
    }

    [Fact]
    public void Should_throw_when_the_owner_has_a_cancel_handler_of_its_own_and_shares_one()
    {
        // Arrange
        var parentBuilder = ParentBuilder.WithCancelHandler<TestCancelHandler>();

        // Act
        var act = () => parentBuilder.WithSharedHandlers(handlers => handlers.WithCancelHandler<TestCancelAndResetHandler>(), out _);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*'{typeof(ParentControlPanel)}'*cancel handler*'{typeof(TestCancelAndResetHandler)}'*");
    }

    [Fact]
    public void Should_throw_when_the_owner_has_a_reset_handler_of_its_own_and_shares_one()
    {
        // Arrange
        var parentBuilder = ParentBuilder.WithResetHandler<TestResetHandler>();

        // Act
        var act = () => parentBuilder.WithSharedHandlers(handlers => handlers.WithResetHandler<TestCancelAndResetHandler>(), out _);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*'{typeof(ParentControlPanel)}'*reset handler*'{typeof(TestCancelAndResetHandler)}'*");
    }

    [Fact]
    public void Should_register_nothing_of_a_shared_handler_the_owner_cannot_share()
    {
        // Arrange
        var parentBuilder = ParentBuilder.WithSaveHandler<SecondSaveHandler>();

        // Act
        var act = () => parentBuilder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out _);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _services.Should().NotContain(d => d.ServiceType == typeof(FirstSaveHandler));
        _services.Should().ContainSingle(d => d.ServiceType == typeof(IControlPanelSaveHandler<TestControlPanelState>)
            && Equals(d.ServiceKey, s_parentKey));
    }

    [Fact]
    public void Should_keep_the_shared_handler_of_the_owner_when_it_configures_one_of_its_own_afterwards()
    {
        // Arrange
        ParentBuilder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out var sharedHandlers)
            .WithSaveHandler<SecondSaveHandler>();
        TabBuilder.WithSharedHandlers(sharedHandlers);

        BuildScope();

        // Act
        var parentSaveHandler = GetSaveHandler(s_parentKey);

        // Assert
        parentSaveHandler.Should().BeAssignableTo<ISharedControlPanelHandler>()
            .Which.HandlerType.Should().Be<FirstSaveHandler>();
    }

    [Fact]
    public void Should_share_the_handlers_of_the_operations_the_owner_has_no_handler_of_its_own_for()
    {
        // Arrange
        ParentBuilder.WithSaveHandler<SecondSaveHandler>();

        // Act
        ShareHandlers(handlers => handlers.WithResetHandler<TestResetHandler>());

        // Assert
        GetSaveHandler(s_parentKey).Should().BeOfType<SecondSaveHandler>();
        GetResetHandler(s_parentKey).Should().BeAssignableTo<ISharedControlPanelHandler>()
            .Which.HandlerType.Should().Be<TestResetHandler>();
    }

    [Fact]
    public void Should_register_a_shared_handler_once_keyed_by_its_owner()
    {
        // Act
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());

        // Assert
        var descriptor = _services.Should().ContainSingle(d => d.ServiceType == typeof(FirstSaveHandler)).Subject;

        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
        descriptor.ServiceKey.Should().Be(s_parentKey);
    }

    [Fact]
    public async Task Should_save_the_control_panels_sharing_a_save_handler_once_when_saved_together()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());
        Invocations.Hold = true;

        // Act
        var parentSave = GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), CancellationToken.None);
        var tabSave = GetSaveHandler(s_tabKey).Save(new TestControlPanelState(), CancellationToken.None);

        Invocations.Release.SetResult();

        // Assert
        (await parentSave).Should().BeOfType<SaveSuccessResult>();
        (await tabSave).Should().BeOfType<SaveSuccessResult>();

        Invocations.Handlers.Should().ContainSingle();
    }

    [Fact]
    public async Task Should_use_one_instance_of_a_shared_handler_per_owner()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());

        // Act
        await GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), CancellationToken.None);
        await GetSaveHandler(s_tabKey).Save(new TestControlPanelState(), CancellationToken.None);

        // Assert
        Invocations.Handlers.Should().HaveCount(2).And.OnlyContain(h => h == Invocations.Handlers[0]);
    }

    [Fact]
    public async Task Should_share_nothing_between_control_panels_configuring_the_same_handler_type_on_their_own()
    {
        // Arrange
        ParentBuilder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out _);
        TabBuilder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out _);
        BuildScope();

        Invocations.Hold = true;

        // Act
        var parentSave = GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), CancellationToken.None);
        var tabSave = GetSaveHandler(s_tabKey).Save(new TestControlPanelState(), CancellationToken.None);

        Invocations.Release.SetResult();
        await Task.WhenAll(parentSave, tabSave);

        // Assert
        Invocations.Handlers.Should().HaveCount(2);
        Invocations.Handlers[1].Should().NotBeSameAs(Invocations.Handlers[0]);
    }

    [Fact]
    public async Task Should_not_join_invocations_of_different_handlers_of_an_owner()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>().WithResetHandler<TestResetHandler>());
        Invocations.Hold = true;

        // Act
        var save = GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), CancellationToken.None);
        var reset = GetResetHandler(s_tabKey).Reset(new TestControlPanelState(), CancellationToken.None);

        Invocations.Release.SetResult();
        await Task.WhenAll(save, reset);

        // Assert
        Invocations.Handlers.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_keep_saving_for_the_others_when_one_control_panel_stops_waiting()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());
        Invocations.Hold = true;

        using var cancellationTokenSource = new CancellationTokenSource();

        var canceledSave = GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), cancellationTokenSource.Token);
        var otherSave = GetSaveHandler(s_tabKey).Save(new TestControlPanelState(), CancellationToken.None);

        // Act
        await cancellationTokenSource.CancelAsync();
        Invocations.Release.SetResult();

        // Assert
        var awaitCanceledSave = () => canceledSave;
        await awaitCanceledSave.Should().ThrowAsync<OperationCanceledException>();

        (await otherSave).Should().BeOfType<SaveSuccessResult>();
        Invocations.Handlers.Should().ContainSingle();
        Invocations.CanceledHandlers.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_cancel_the_shared_invocation_once_every_control_panel_stopped_waiting()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());
        Invocations.Hold = true;

        using var parentCancellationTokenSource = new CancellationTokenSource();
        using var tabCancellationTokenSource = new CancellationTokenSource();

        var parentSave = GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), parentCancellationTokenSource.Token);
        var tabSave = GetSaveHandler(s_tabKey).Save(new TestControlPanelState(), tabCancellationTokenSource.Token);

        // Act
        await parentCancellationTokenSource.CancelAsync();
        var canceledHandlersAfterFirstLeft = Invocations.CanceledHandlers.Count;

        await tabCancellationTokenSource.CancelAsync();

        // Assert
        var awaitParentSave = () => parentSave;
        await awaitParentSave.Should().ThrowAsync<OperationCanceledException>();

        var awaitTabSave = () => tabSave;
        await awaitTabSave.Should().ThrowAsync<OperationCanceledException>();

        canceledHandlersAfterFirstLeft.Should().Be(0);
        Invocations.CanceledHandlers.Should().ContainSingle();
    }

    [Fact]
    public async Task Should_start_a_new_invocation_after_the_running_one_was_canceled()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>());
        Invocations.Hold = true;

        using var cancellationTokenSource = new CancellationTokenSource();

        var canceledSave = GetSaveHandler(s_parentKey).Save(new TestControlPanelState(), cancellationTokenSource.Token);
        await cancellationTokenSource.CancelAsync();

        var awaitCanceledSave = () => canceledSave;
        await awaitCanceledSave.Should().ThrowAsync<OperationCanceledException>();

        // Act
        var nextSave = GetSaveHandler(s_tabKey).Save(new TestControlPanelState(), CancellationToken.None);
        Invocations.Release.SetResult();

        // Assert
        (await nextSave).Should().BeOfType<SaveSuccessResult>();
        Invocations.Handlers.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_not_join_a_cancel_and_a_reset_of_a_handler_implementing_both()
    {
        // Arrange
        ShareHandlers(handlers => handlers
            .WithCancelHandler<TestCancelAndResetHandler>()
            .WithResetHandler<TestCancelAndResetHandler>());
        Invocations.Hold = true;

        // Act
        var cancel = GetCancelHandler(s_parentKey).Cancel(new TestControlPanelState(), CancellationToken.None);
        var reset = GetResetHandler(s_tabKey).Reset(new TestControlPanelState(), CancellationToken.None);

        Invocations.Release.SetResult();
        await Task.WhenAll(cancel, reset);

        // Assert
        Invocations.Handlers.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_cancel_the_control_panels_sharing_a_cancel_handler_once_when_canceled_together()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithCancelHandler<TestCancelHandler>());
        Invocations.Hold = true;

        // Act
        var parentCancel = GetCancelHandler(s_parentKey).Cancel(new TestControlPanelState(), CancellationToken.None);
        var tabCancel = GetCancelHandler(s_tabKey).Cancel(new TestControlPanelState(), CancellationToken.None);

        Invocations.Release.SetResult();
        await Task.WhenAll(parentCancel, tabCancel);

        // Assert
        Invocations.Handlers.Should().ContainSingle();
        GetCancelHandler(s_tabKey).Should().BeAssignableTo<ISharedControlPanelHandler>()
            .Which.HandlerType.Should().Be<TestCancelHandler>();
    }

    [Fact]
    public async Task Should_reset_the_control_panels_sharing_a_reset_handler_once_when_reset_together()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithResetHandler<TestResetHandler>());
        Invocations.Hold = true;

        // Act
        var parentReset = GetResetHandler(s_parentKey).Reset(new TestControlPanelState(), CancellationToken.None);
        var tabReset = GetResetHandler(s_tabKey).Reset(new TestControlPanelState(), CancellationToken.None);

        Invocations.Release.SetResult();
        await Task.WhenAll(parentReset, tabReset);

        // Assert
        Invocations.Handlers.Should().ContainSingle();
    }

    [Fact]
    public async Task Should_reset_again_after_the_previous_reset_ended()
    {
        // Arrange
        ShareHandlers(handlers => handlers.WithResetHandler<TestResetHandler>());

        // Act
        await GetResetHandler(s_parentKey).Reset(new TestControlPanelState(), CancellationToken.None);
        await GetResetHandler(s_tabKey).Reset(new TestControlPanelState(), CancellationToken.None);

        // Assert
        Invocations.Handlers.Should().HaveCount(2);
    }

    [Fact]
    public void Should_keep_a_handler_the_control_panel_configured_already()
    {
        // Arrange
        ParentBuilder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out var sharedHandlers);
        TabBuilder.WithSaveHandler<SecondSaveHandler>().WithSharedHandlers(sharedHandlers);
        BuildScope();

        // Act
        var tabSaveHandler = GetSaveHandler(s_tabKey);

        // Assert
        tabSaveHandler.Should().BeOfType<SecondSaveHandler>();
    }

    [Fact]
    public void Should_not_support_shared_handlers_on_a_builder_other_than_the_sdks()
    {
        // Arrange
        IControlPanelBuilder<TestClientModule, ParentControlPanel, TestControlPanelState> builder = new OtherControlPanelBuilder();

        // Act
        var act = () => builder.WithSharedHandlers(handlers => handlers.WithSaveHandler<FirstSaveHandler>(), out _);

        // Assert
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Should_not_support_shared_handlers_not_created_by_the_sdk()
    {
        // Act
        var act = () => TabBuilder.WithSharedHandlers(new OtherSharedHandlers());

        // Assert
        act.Should().Throw<NotSupportedException>();
    }

    private void ShareHandlers(Action<ISharedControlPanelHandlersBuilder<TestControlPanelState>> configureSharedHandlers)
    {
        ParentBuilder.WithSharedHandlers(configureSharedHandlers, out var sharedHandlers);
        TabBuilder.WithSharedHandlers(sharedHandlers);

        BuildScope();
    }

    private void BuildScope()
    {
        _serviceProvider = _services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        _scope = _serviceProvider.CreateAsyncScope();
    }

    private IControlPanelSaveHandler<TestControlPanelState> GetSaveHandler(Type key)
        => _scope.ServiceProvider.GetRequiredKeyedService<IControlPanelSaveHandler<TestControlPanelState>>(key);

    private IControlPanelCancelHandler<TestControlPanelState> GetCancelHandler(Type key)
        => _scope.ServiceProvider.GetRequiredKeyedService<IControlPanelCancelHandler<TestControlPanelState>>(key);

    private IControlPanelResetHandler<TestControlPanelState> GetResetHandler(Type key)
        => _scope.ServiceProvider.GetRequiredKeyedService<IControlPanelResetHandler<TestControlPanelState>>(key);
}
