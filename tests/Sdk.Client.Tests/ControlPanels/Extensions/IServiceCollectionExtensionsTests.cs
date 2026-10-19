using System.Collections;
using AwesomeAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels.Attributes;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Extensions;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;
using Sdk.Client.Services;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels.Extensions;

public sealed class IServiceCollectionExtensionsTests
{
    internal sealed class TestControlPanelState : ControlPanelState;

    internal sealed class TestCategoryDescriptor : IControlPanelCategoryDescriptor
    {
        public string Title => "Test category";
    }

    internal sealed class RootParentDescriptor : IControlPanelParentDescriptor;

    internal sealed class MidParentDescriptor : IControlPanelParentDescriptor;

    internal sealed class DanglingParentDescriptor : IControlPanelParentDescriptor;

    internal sealed class NoCategoryParentDescriptor : IControlPanelParentDescriptor;

    internal sealed class ChildOfNoCategoryParentDescriptor : IControlPanelParentDescriptor;

    [ControlPanelCategory<TestCategoryDescriptor>]
    [ControlPanelParent<RootParentDescriptor>]
    internal sealed class RootControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class RootControlPanelDescriptor : IControlPanelDescriptor<RootControlPanel>
    {
        public string Title => "Root";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<TestCategoryDescriptor>]
    [ControlPanelChild<RootParentDescriptor>]
    [ControlPanelParent<MidParentDescriptor>]
    internal sealed class MidControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class MidControlPanelDescriptor : IControlPanelDescriptor<MidControlPanel>
    {
        public string Title => "Mid";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<TestCategoryDescriptor>]
    [ControlPanelChild<MidParentDescriptor>]
    internal sealed class LeafControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class LeafControlPanelDescriptor : IControlPanelDescriptor<LeafControlPanel>
    {
        public string Title => "Leaf";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<TestCategoryDescriptor>]
    internal sealed class OtherRootControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class OtherRootControlPanelDescriptor : IControlPanelDescriptor<OtherRootControlPanel>
    {
        public string Title => "Other root";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<TestCategoryDescriptor>]
    [ControlPanelChild<DanglingParentDescriptor>]
    internal sealed class DanglingControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class DanglingControlPanelDescriptor : IControlPanelDescriptor<DanglingControlPanel>
    {
        public string Title => "Dangling";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelParent<NoCategoryParentDescriptor>]
    internal sealed class NoCategoryParentControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class NoCategoryParentControlPanelDescriptor : IControlPanelDescriptor<NoCategoryParentControlPanel>
    {
        public string Title => "No category parent";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<TestCategoryDescriptor>]
    [ControlPanelChild<NoCategoryParentDescriptor>]
    [ControlPanelParent<ChildOfNoCategoryParentDescriptor>]
    internal sealed class ChildOfNoCategoryParentControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class ChildOfNoCategoryParentControlPanelDescriptor : IControlPanelDescriptor<ChildOfNoCategoryParentControlPanel>
    {
        public string Title => "Child of no category parent";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    [ControlPanelCategory<TestCategoryDescriptor>]
    [ControlPanelChild<ChildOfNoCategoryParentDescriptor>]
    internal sealed class GrandchildOfNoCategoryParentControlPanel : ControlPanelBase<TestControlPanelState>;

    internal sealed class GrandchildOfNoCategoryParentControlPanelDescriptor : IControlPanelDescriptor<GrandchildOfNoCategoryParentControlPanel>
    {
        public string Title => "Grandchild of no category parent";
        public Uri IconUrl => new("https://example.com/icon.png");
    }

    private static ServiceCollection CreateServices(Action<IServiceCollection> registerControlPanels)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IControlPanelRegistryFactory, FakeControlPanelRegistryFactory>();

        registerControlPanels(services);

        return services;
    }

    [Fact]
    public void Should_link_child_to_parent_when_parent_is_registered_before_child()
    {
        // Arrange
        var services = CreateServices(s =>
        {
            s.AddControlPanel<TestClientModule, RootControlPanel, TestControlPanelState>().WithAutoDiscovery<RootControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, MidControlPanel, TestControlPanelState>().WithAutoDiscovery<MidControlPanelDescriptor>();
        });

        // Act
        var registry = services.BuildServiceProvider().GetRequiredService<IControlPanelRegistry<TestClientModule>>();

        // Assert
        var root = registry.Single(i => i.ComponentType == typeof(RootControlPanel));
        var mid = registry.Single(i => i.ComponentType == typeof(MidControlPanel));

        root.ChildOf.Should().BeNull();
        mid.ChildOf.Should().BeSameAs(root.AsParent);
    }

    [Fact]
    public void Should_link_child_to_parent_when_child_is_registered_before_parent()
    {
        // Arrange
        var services = CreateServices(s =>
        {
            s.AddControlPanel<TestClientModule, MidControlPanel, TestControlPanelState>().WithAutoDiscovery<MidControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, RootControlPanel, TestControlPanelState>().WithAutoDiscovery<RootControlPanelDescriptor>();
        });

        // Act
        var registry = services.BuildServiceProvider().GetRequiredService<IControlPanelRegistry<TestClientModule>>();

        // Assert
        var root = registry.Single(i => i.ComponentType == typeof(RootControlPanel));
        var mid = registry.Single(i => i.ComponentType == typeof(MidControlPanel));

        mid.ChildOf.Should().BeSameAs(root.AsParent);
    }

    [Fact]
    public void Should_link_a_three_level_chain_where_the_middle_panel_is_both_a_child_and_a_parent()
    {
        // Arrange
        var services = CreateServices(s =>
        {
            s.AddControlPanel<TestClientModule, LeafControlPanel, TestControlPanelState>().WithAutoDiscovery<LeafControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, RootControlPanel, TestControlPanelState>().WithAutoDiscovery<RootControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, MidControlPanel, TestControlPanelState>().WithAutoDiscovery<MidControlPanelDescriptor>();
        });

        // Act
        var registry = services.BuildServiceProvider().GetRequiredService<IControlPanelRegistry<TestClientModule>>();

        // Assert
        var root = registry.Single(i => i.ComponentType == typeof(RootControlPanel));
        var mid = registry.Single(i => i.ComponentType == typeof(MidControlPanel));
        var leaf = registry.Single(i => i.ComponentType == typeof(LeafControlPanel));

        mid.ChildOf.Should().BeSameAs(root.AsParent);
        leaf.ChildOf.Should().BeSameAs(mid.AsParent);
    }

    [Fact]
    public void Should_register_a_child_whose_parent_is_missing_entirely()
    {
        // Arrange - DanglingControlPanel declares a child attribute for a parent descriptor type no
        // control panel provides. Ordering is no concern of the registry, so it is registered like any
        // other control panel and keeps the key it names; resolving it is up to the consumer.
        var services = CreateServices(s =>
            s.AddControlPanel<TestClientModule, DanglingControlPanel, TestControlPanelState>().WithAutoDiscovery<DanglingControlPanelDescriptor>());

        // Act
        var registry = services.BuildServiceProvider().GetRequiredService<IControlPanelRegistry<TestClientModule>>();

        // Assert
        var dangling = registry.Single(i => i.ComponentType == typeof(DanglingControlPanel));

        dangling.ChildOf.Should().BeOfType<DanglingParentDescriptor>();
    }

    [Fact]
    public void Should_register_control_panels_in_registration_order_regardless_of_nesting()
    {
        // Arrange - MidControlPanel is a child of RootControlPanel and registered before it
        var services = CreateServices(s =>
        {
            s.AddControlPanel<TestClientModule, OtherRootControlPanel, TestControlPanelState>().WithAutoDiscovery<OtherRootControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, MidControlPanel, TestControlPanelState>().WithAutoDiscovery<MidControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, RootControlPanel, TestControlPanelState>().WithAutoDiscovery<RootControlPanelDescriptor>();
        });

        // Act
        var registry = services.BuildServiceProvider().GetRequiredService<IControlPanelRegistry<TestClientModule>>();

        // Assert - no reordering: a child no longer has to wait for its parent
        registry.Select(i => i.ComponentType).Should().Equal(typeof(OtherRootControlPanel), typeof(MidControlPanel), typeof(RootControlPanel));
    }

    [Fact]
    public void Should_not_cascade_a_missing_category_descriptor_to_the_children_of_that_control_panel()
    {
        // Arrange - NoCategoryParentControlPanel cannot be registered, having no category descriptor. Its
        // child and grandchild no longer depend on it being registered, so they are registered themselves
        // and keep the keys they name.
        var services = CreateServices(s =>
        {
            s.AddControlPanel<TestClientModule, GrandchildOfNoCategoryParentControlPanel, TestControlPanelState>().WithAutoDiscovery<GrandchildOfNoCategoryParentControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, ChildOfNoCategoryParentControlPanel, TestControlPanelState>().WithAutoDiscovery<ChildOfNoCategoryParentControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, NoCategoryParentControlPanel, TestControlPanelState>().WithAutoDiscovery<NoCategoryParentControlPanelDescriptor>();
            s.AddControlPanel<TestClientModule, RootControlPanel, TestControlPanelState>().WithAutoDiscovery<RootControlPanelDescriptor>();
        });

        // Act
        var registry = services.BuildServiceProvider().GetRequiredService<IControlPanelRegistry<TestClientModule>>();

        // Assert
        registry.Select(i => i.ComponentType).Should().Equal(
            typeof(GrandchildOfNoCategoryParentControlPanel), typeof(ChildOfNoCategoryParentControlPanel), typeof(RootControlPanel));

        var child = registry.Single(i => i.ComponentType == typeof(ChildOfNoCategoryParentControlPanel));
        var grandchild = registry.Single(i => i.ComponentType == typeof(GrandchildOfNoCategoryParentControlPanel));

        child.ChildOf.Should().BeOfType<NoCategoryParentDescriptor>();
        grandchild.ChildOf.Should().BeSameAs(child.AsParent);
    }

    private sealed class FakeControlPanelRegistryFactory : IControlPanelRegistryFactory
    {
        public IControlPanelRegistry<TClientModule> CreateControlPanelRegistry<TClientModule>()
            where TClientModule : class, IClientModule
            => new FakeControlPanelRegistry<TClientModule>();
    }

    private sealed class FakeControlPanelRegistry<TClientModule> : IControlPanelRegistry<TClientModule>
        where TClientModule : class, IClientModule
    {
        private readonly List<IControlPanelRegistryItem> _items = [];

        public int UpdateLock => 0;

#pragma warning disable CS0067
        public event Action<RegistryChangedEventArgs<IControlPanelRegistryItem>>? Changed;
#pragma warning restore CS0067

        public IControlPanelRegistryItem Add<TComponent, TState>(IControlPanelDescriptor descriptor, TState state,
            IControlPanelCategoryDescriptor categoryDescriptor, IControlPanelGroupDescriptor? groupDescriptor = null,
            IAuthorizationRequirement? authorizationRequirement = null,
            IControlPanelParentDescriptor? childOf = null, IControlPanelParentDescriptor? asParent = null)
                where TComponent : ControlPanelBase<TState>
                where TState : IControlPanelState
        {
            var item = new FakeControlPanelRegistryItem
            {
                ComponentType = typeof(TComponent),
                Descriptor = descriptor,
                CategoryDescriptor = categoryDescriptor,
                GroupDescriptor = groupDescriptor!,
                State = state,
                AuthorizationRequirement = authorizationRequirement,
                ChildOf = childOf,
                AsParent = asParent
            };

            _items.Add(item);

            return item;
        }

        public int Remove<TControlPanel>()
            where TControlPanel : IControlPanel
            => _items.RemoveAll(item => item.ComponentType == typeof(TControlPanel));

        public bool Remove(IControlPanelRegistryItem item) => _items.Remove(item);

        public int Remove(Predicate<IControlPanelRegistryItem> match) => _items.RemoveAll(match);

        public void BeginUpdate() { }

        public void EndUpdate() { }

        public IEnumerator<IControlPanelRegistryItem> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class FakeControlPanelRegistryItem : IControlPanelRegistryItem
    {
        public required Type ComponentType { get; init; }
        public required IControlPanelDescriptor Descriptor { get; init; }
        public required IControlPanelCategoryDescriptor CategoryDescriptor { get; init; }
        public required IControlPanelGroupDescriptor GroupDescriptor { get; init; }
        public required IControlPanelState State { get; init; }
        public IAuthorizationRequirement? AuthorizationRequirement { get; init; }
        public IControlPanelParentDescriptor? ChildOf { get; init; }
        public IControlPanelParentDescriptor? AsParent { get; init; }
    }
}
