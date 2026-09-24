using AwesomeAssertions;
using Sdk.Client.NotificationArea.Services;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed class NotificationElementStateTests
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void Assert_changed_event_handling_when_is_active_is_set(bool isActiveInitially, bool isActive, bool shouldTriggerChangedEvent)
    {
        // Arrange
        var changedTriggered = false;

        var state = new NotificationElementState { IsActive = isActiveInitially };
        state.Changed += args => changedTriggered = args.PropertyNames.Contains(nameof(NotificationElementState.IsActive));

        // Act
        state.IsActive = isActive;

        // Assert
        changedTriggered.Should().Be(shouldTriggerChangedEvent);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void Assert_changed_event_handling_when_visible_is_set(bool visibleInitially, bool visible, bool shouldTriggerChangedEvent)
    {
        // Arrange
        var changedTriggered = false;

        var state = new NotificationElementState { Visible = visibleInitially };
        state.Changed += args => changedTriggered = args.PropertyNames.Contains(nameof(NotificationElementState.Visible));

        // Act
        state.Visible = visible;

        // Assert
        changedTriggered.Should().Be(shouldTriggerChangedEvent);
    }

    [Fact]
    public void Should_set_is_active_to_false_when_visible_is_set_to_false()
    {
        // Arrange
        var state = new NotificationElementState
        {
            // Act
            Visible = false
        };

        // Assert
        state.IsActive.Should().Be(false);
    }

    [Fact]
    public void Should_set_visible_to_true_when_is_active_is_set_to_true()
    {
        // Arrange
        var state = new NotificationElementState
        {
            Visible = false,         // Act
            IsActive = true
        };

        // Assert
        state.Visible.Should().Be(true);
    }

    [Fact]
    public void Should_not_trigger_changed_event_after_begin_update()
    {
        // Arrange
        var state = new NotificationElementState();

        var stateChanged = false;

        state.Changed += _ => stateChanged = true;

        // Act
        state.BeginUpdate();

        state.Visible = false;
        state.IsActive = true;

        // Assert
        stateChanged.Should().Be(false);
    }

    [Fact]
    public void Should_trigger_changed_event_on_end_update_with_correct_args()
    {
        // Arrange
        var state = new NotificationElementState();

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
            state.IsActive = true;
            state.Visible = false;
        }
        finally
        {
            state.EndUpdate();
        }

        // Assert
        changedCounter.Should().Be(1);
        affectedPropertyNames.Should().HaveCount(2);
    }

    [Fact]
    public void Should_keep_the_property_names_of_end_update_args_after_end_update_returned()
    {
        // Arrange
        var state = new NotificationElementState();

        NotificationElementStateChangedEventArgs? changedArgs = null;
        state.Changed += args => changedArgs = args;

        // Act
        state.BeginUpdate();
        state.IsActive = true;
        state.EndUpdate();

        // Assert
        changedArgs.Should().NotBeNull();
        changedArgs.PropertyNames.Should().BeEquivalentTo([nameof(NotificationElementState.IsActive)]);
    }

    [Fact]
    public void Should_not_hold_the_lock_while_raising_changed_on_end_update()
    {
        // Arrange
        var state = new NotificationElementState();

        var otherThreadEnteredUpdate = false;
        state.Changed += _ => otherThreadEnteredUpdate = Task.Run(state.BeginUpdate).Wait(TimeSpan.FromSeconds(5));

        // Act
        state.BeginUpdate();
        state.IsActive = true;
        state.EndUpdate();

        // Assert
        otherThreadEnteredUpdate.Should().BeTrue();
    }

    [Fact]
    public async Task Should_trigger_changed_event_on_outer_end_update()
    {
        // Arrange
        var stateChangedCounter = 0;

        var state = new NotificationElementState();
        state.Changed += _ => ++stateChangedCounter;

        static async Task RandomUpdateTask(Random random, NotificationElementState state)
        {
            var delay = random.Next(0, 100);
            await Task.Delay(delay);

            state.BeginUpdate();
            try
            {
                var coinToss = random.Next(0, 2);

                if (coinToss == 0)
                    state.Visible = !state.Visible;
                else
                    state.IsActive = !state.IsActive;
            }
            finally
            {
                state.EndUpdate();
            }
        }

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
    }
}
