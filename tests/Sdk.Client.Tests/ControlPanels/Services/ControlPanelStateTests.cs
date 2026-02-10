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
        controlPanelState.Changed += _ => changedTriggered = true;

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

        controlPanelState.Changed += _ => changedTriggered = true;

        // Act
        controlPanelState.EndLoading();

        // Assert
        changedTriggered.Should().Be(shouldTriggerChangedEvent);
    }

    [Fact]
    public void Should_not_trigger_changed_event_after_begin_update()
    {
        // Arrange
        var state = new TestControlPanelState();

        var stateChanged = false;

        state.Changed += _ => stateChanged = true;

        // Act
        state.BeginUpdate();

        state.BeginLoading();
        try
        {
            state.Bar++;
            state.Foo = false;
        }
        finally
        {
            state.EndLoading();
        }

        // Assert
        stateChanged.Should().Be(false);
    }

    [Fact]
    public void Should_trigger_changed_event_on_end_update_with_correct_args()
    {
        // Arrange
        var state = new TestControlPanelState();

        var changedCounter = 0;
        var affectedPropertyNames = new List<string>();

        state.Changed += args =>
        {
            changedCounter++;
            affectedPropertyNames.AddRange(args.PropertyNames);
        };

        // Act
        state.BeginUpdate();
        try
        {
            state.BeginLoading();
            try
            {
                state.Foo = true;
                state.Bar--;
            }
            finally
            {
                state.EndLoading();
            }
        }
        finally
        {
            state.EndUpdate();
        }

        // Assert
        changedCounter.Should().Be(1);
        affectedPropertyNames.Should().BeEquivalentTo([nameof(state.IsLoading), nameof(state.Foo), nameof(state.Bar)]);
    }

    [Fact]
    public async Task Should_trigger_changed_event_on_outer_end_update()
    {
        // Arrange
        var stateChangedCounter = 0;

        var state = new TestControlPanelState();
        state.Changed += _ => ++stateChangedCounter;

        // Act
        state.BeginUpdate();
        try
        {
            var random = new Random();

            var updateTasks = new List<Task>();
            for (var i = 0; i < 1000; i++)
                updateTasks.Add(RandomUpdateTask(random, state));

            await Task.WhenAll(updateTasks);
        }
        finally
        {
            state.EndUpdate();
        }

        // Assert
        stateChangedCounter.Should().Be(1);
        return;

        static async Task RandomUpdateTask(Random random, TestControlPanelState state)
        {
            var delay = random.Next(0, 100);
            await Task.Delay(delay, TestContext.Current.CancellationToken);

            state.BeginUpdate();
            try
            {
                state.BeginLoading();
                try
                {
                    var coinToss = random.Next(0, 2);

                    if (coinToss == 0)
                    {
                        state.Foo = true;
                        state.Bar--;
                    }
                    else
                    {
                        state.Bar++;
                    }
                }
                finally
                {
                    state.EndLoading();
                }
            }
            finally
            {
                state.EndUpdate();
            }
        }
    }

    [Fact]
    public void Should_increase_update_lock_on_begin_update()
    {
        // Arrange
        var state = new TestControlPanelState();

        // Act
        state.BeginUpdate();

        // Assert
        state.UpdateLock.Should().Be(1);
    }

    [Fact]
    public void Should_decrease_update_lock_on_end_update()
    {
        // Arrange
        var state = new TestControlPanelState();

        // Act
        state.BeginUpdate();
        state.EndUpdate();

        // Assert
        state.UpdateLock.Should().Be(0);
    }


    public class TestControlPanelState : ControlPanelState
    {
        public bool Foo
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    OnPropertyChanged();
                }
            }
        }

        public int? Bar
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    OnPropertyChanged();
                }
            }
        } = 0;
    }

}
