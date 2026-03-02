using AwesomeAssertions;
using Sdk.Client.NavTiles.Components;
using Xunit;

namespace Sdk.Client.Tests.NavTiles;

public sealed class NavTileStateTests
{
    public sealed class EnabledProperty
    {
        [Fact]
        public void Should_fire_changed_event_when_set_to_new_value()
        {
            var state = new NavTileState();
            var changed = false;
            state.Changed += () => changed = true;

            state.Enabled = true;

            changed.Should().BeTrue();
        }

        [Fact]
        public void Should_not_fire_changed_event_when_set_to_same_value()
        {
            var state = new NavTileState { Enabled = true };
            var changed = false;
            state.Changed += () => changed = true;

            state.Enabled = true;

            changed.Should().BeFalse();
        }
    }

    public sealed class LinkTargetProperty
    {
        [Fact]
        public void Should_fire_changed_event_when_set_to_new_value()
        {
            var state = new NavTileState();
            var changed = false;
            state.Changed += () => changed = true;

            state.LinkTarget = "/new-page";

            changed.Should().BeTrue();
            state.LinkTarget.Should().Be("/new-page");
        }

        [Fact]
        public void Should_not_fire_changed_event_when_set_to_same_value()
        {
            var state = new NavTileState { LinkTarget = "/page" };
            var changed = false;
            state.Changed += () => changed = true;

            state.LinkTarget = "/page";

            changed.Should().BeFalse();
        }
    }

    public sealed class Loading
    {
        [Fact]
        public void BeginLoading_should_set_is_loading_to_true()
        {
            var state = new NavTileState();

            state.BeginLoading();

            state.IsLoading.Should().BeTrue();
        }

        [Fact]
        public void BeginLoading_should_fire_changed_event()
        {
            var state = new NavTileState();
            var changed = false;
            state.Changed += () => changed = true;

            state.BeginLoading();

            changed.Should().BeTrue();
        }

        [Fact]
        public void EndLoading_should_set_is_loading_to_false_after_last_operation()
        {
            var state = new NavTileState();
            state.BeginLoading();

            state.EndLoading();

            state.IsLoading.Should().BeFalse();
        }

        [Fact]
        public void Should_remain_loading_when_multiple_operations_are_active()
        {
            var state = new NavTileState();
            state.BeginLoading();
            state.BeginLoading();

            state.EndLoading();

            state.IsLoading.Should().BeTrue();
        }

        [Fact]
        public void Should_stop_loading_when_all_operations_end()
        {
            var state = new NavTileState();
            state.BeginLoading();
            state.BeginLoading();

            state.EndLoading();
            state.EndLoading();

            state.IsLoading.Should().BeFalse();
        }

        [Fact]
        public void EndLoading_should_fire_changed_event_when_loading_stops()
        {
            var state = new NavTileState();
            state.BeginLoading();
            var changed = false;
            state.Changed += () => changed = true;

            state.EndLoading();

            changed.Should().BeTrue();
        }

        [Fact]
        public void EndLoading_should_not_go_below_zero()
        {
            var state = new NavTileState();

            state.EndLoading();
            state.EndLoading();

            state.IsLoading.Should().BeFalse();
        }
    }
}

