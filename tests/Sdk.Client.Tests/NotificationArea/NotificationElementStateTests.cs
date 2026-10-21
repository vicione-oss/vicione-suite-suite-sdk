using System.Security.Cryptography;
using AwesomeAssertions;
using Sdk.Client.NotificationArea.Services;
using ViciOne.SourceGenerators.NotifyChanged;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed partial class NotificationElementStateTests
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
    public void Should_be_visible_and_inactive_by_default()
    {
        // Arrange
        NotificationElementState state;

        // Act
        state = new NotificationElementState();

        // Assert
        state.Visible.Should().Be(true);
        state.IsActive.Should().Be(false);
    }

    [Fact]
    public void Should_deactivate_first_and_notify_both_when_an_active_element_is_hidden()
    {
        // Arrange
        var changes = new List<(string Name, bool IsActive, bool Visible)>();

        var state = new NotificationElementState { IsActive = true };
        state.Changed += args => changes.Add((args.PropertyNames.Single(), state.IsActive, state.Visible));

        // Act
        state.Visible = false;

        // Assert
        changes.Should().Equal(
            (nameof(NotificationElementState.IsActive), false, false),
            (nameof(NotificationElementState.Visible), false, false));
    }

    [Fact]
    public void Should_show_first_and_notify_both_when_a_hidden_element_is_activated()
    {
        // Arrange
        var changes = new List<(string Name, bool IsActive, bool Visible)>();

        var state = new NotificationElementState { Visible = false };
        state.Changed += args => changes.Add((args.PropertyNames.Single(), state.IsActive, state.Visible));

        // Act
        state.IsActive = true;

        // Assert
        changes.Should().Equal(
            (nameof(NotificationElementState.Visible), false, true),
            (nameof(NotificationElementState.IsActive), true, true));
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

        static async Task RandomUpdateTask(NotificationElementState state)
        {
            var delay = RandomNumberGenerator.GetInt32(0, 100);
            await Task.Delay(delay);

            state.BeginUpdate();
            try
            {
                var coinToss = RandomNumberGenerator.GetInt32(0, 2);

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
            var updateTasks = new List<Task>();
            for (var i = 0; i < 1000; i++)
                updateTasks.Add(RandomUpdateTask(state));

            await Task.WhenAll(updateTasks);
        }
        finally
        {
            state.EndUpdate();
        }

        // Assert
        stateChangedCounter.Should().Be(1);
    }

    [Fact]
    public void Should_raise_changed_for_a_notify_changed_property_of_a_derived_state()
    {
        // Arrange
        var changes = new List<string[]>();

        var state = new DerivedNotificationElementState();
        state.Changed += args => changes.Add([.. args.PropertyNames]);

        // Act
        state.UnreadCount = 3;

        // Assert
        changes.Should().ContainSingle().Which.Should().Equal(nameof(DerivedNotificationElementState.UnreadCount));
    }

    private sealed partial class DerivedNotificationElementState : NotificationElementState
    {
        [NotifyChanged]
        public partial int UnreadCount { get; set; }
    }
}
