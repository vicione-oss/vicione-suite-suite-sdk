using AwesomeAssertions;
using Sdk.Client.ControlPanels.Services;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels.Services;

public sealed class ControlPanelStateTests
{
    [Theory]
    [InlineData(null, null, false)]
    [InlineData(null, 1, true)]
    [InlineData(1, null, true)]
    [InlineData(1, 1, false)]
    public void Assert_changed_event_handling_when_is_active_control_panel_page_index_is_set(int? activeControlPanelPageIndexInitially, int? activeControlPanelPageIndex, bool shouldTriggerChangedEvent)
    {
        // Arrange
        var changedTriggered = false;

        var state = new ControlPanelState { ActivePageIndex = activeControlPanelPageIndexInitially };
        state.Changed += args => changedTriggered = args.PropertyNames.Contains(nameof(args.Sender.ActivePageIndex));

        // Act
        state.ActivePageIndex = activeControlPanelPageIndex;

        // Assert
        changedTriggered.Should().Be(shouldTriggerChangedEvent);
    }

    [Fact]
    public void Should_indicate_loading_after_begin_loading()
    {
        // Arrange
        var controlPanelState = new ControlPanelState();

        // Act
        controlPanelState.BeginLoading();

        // Assert
        controlPanelState.IsLoading.Should().BeTrue();
    }

    [Fact]
    public void Should_not_indicate_loading_after_end_loading()
    {
        // Arrange
        var controlPanelState = new ControlPanelState();

        // Act
        controlPanelState.EndLoading();

        // Assert
        controlPanelState.IsLoading.Should().BeFalse();
    }

    [Fact]
    public void Should_indicate_loading_after_incomplete_loading_call_sequence()
    {
        // Arrange
        var controlPanelState = new ControlPanelState();

        // Act
        controlPanelState.BeginLoading();
        controlPanelState.BeginLoading();
        controlPanelState.EndLoading();

        // Assert
        controlPanelState.IsLoading.Should().BeTrue();
    }

    [Fact]
    public void Should_not_indicate_loading_after_complete_loading_call_sequence()
    {
        // Arrange
        var controlPanelState = new ControlPanelState();

        // Act
        controlPanelState.BeginLoading();
        controlPanelState.BeginLoading();
        controlPanelState.EndLoading();
        controlPanelState.EndLoading();

        // Assert
        controlPanelState.IsLoading.Should().BeFalse();
    }

    [Fact]
    public void Should_trigger_changed_event_on_begin_loading()
    {
        // Arrange
        var changedTriggered = false;

        var controlPanelState = new ControlPanelState();
        controlPanelState.Changed += args => changedTriggered = true;

        // Act
        controlPanelState.BeginLoading();

        // Assert
        changedTriggered.Should().Be(true);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void Assert_changed_event_handling_on_end_loading(bool isLoading, bool shouldTriggerChangedEvent)
    {
        // Arrange
        var changedTriggered = false;

        var controlPanelState = new ControlPanelState();

        if (isLoading)
            controlPanelState.BeginLoading();

        controlPanelState.Changed += args => changedTriggered = true;

        // Act
        controlPanelState.EndLoading();

        // Assert
        changedTriggered.Should().Be(shouldTriggerChangedEvent);
    }
}
