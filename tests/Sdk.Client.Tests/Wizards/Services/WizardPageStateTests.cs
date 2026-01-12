using AwesomeAssertions;
using Sdk.Client.Wizards.Models;
using Sdk.Client.Wizards.Services;
using Xunit;

namespace Sdk.Client.Tests.Wizards.Services;

public sealed class WizardPageStateTests
{
    [Fact]
    public void Should_indicate_current_operation_after_begin_operation()
    {
        // Arrange
        var wizardPageState = new WizardPageState();
        var wizardOperation = new WizardOperation();

        // Act
        wizardPageState.BeginOperation(wizardOperation);

        // Assert
        wizardPageState.CurrentOperation.Should().Be(wizardOperation);
    }

    [Fact]
    public void Should_not_indicate_current_operation_after_end_operation()
    {
        // Arrange
        var wizardPageState = new WizardPageState();
        var wizardOperation = new WizardOperation();

        wizardPageState.BeginOperation(wizardOperation);

        // Act
        wizardPageState.EndOperation();

        // Assert
        wizardPageState.CurrentOperation.Should().BeNull();
    }

    [Fact]
    public void Should_indicate_current_operation_after_incomplete_begin_end_call_sequence()
    {
        // Arrange
        var wizardPageState = new WizardPageState();
        var wizardOperation1 = new WizardOperation();

        // Act
        wizardPageState.BeginOperation(wizardOperation1);
        wizardPageState.BeginOperation(new WizardOperation());
        wizardPageState.EndOperation();

        // Assert
        wizardPageState.CurrentOperation.Should().Be(wizardOperation1);
    }

    [Fact]
    public void Should_not_indicate_current_operation_after_complete_begin_end_call_sequence()
    {
        // Arrange
        var wizardPageState = new WizardPageState();

        // Act
        wizardPageState.BeginOperation(new WizardOperation());
        wizardPageState.BeginOperation(new WizardOperation());
        wizardPageState.EndOperation();
        wizardPageState.EndOperation();

        // Assert
        wizardPageState.CurrentOperation.Should().BeNull();
    }

    [Fact]
    public void Should_trigger_changed_event_on_begin_operation()
    {
        // Arrange
        var changedTriggered = false;

        var wizardPageState = new WizardPageState();
        wizardPageState.Changed += _ => changedTriggered = true;

        // Act
        wizardPageState.BeginOperation(new WizardOperation());

        // Assert
        changedTriggered.Should().Be(true);
    }

    [Fact]
    public void Should_not_trigger_changed_event_on_end_operation()
    {
        // Arrange
        var changedTriggered = false;

        var wizardPageState = new WizardPageState();
        wizardPageState.BeginOperation(new WizardOperation());
        wizardPageState.Changed += _ => changedTriggered = true;

        // Act
        wizardPageState.EndOperation();

        // Assert
        changedTriggered.Should().Be(true);
    }

    [Fact]
    public async Task Should_trigger_changed_event_on_outer_end_operation()
    {
        // Arrange
        var wizardPageStateChangedCounter = 0;

        var wizardPageState = new WizardPageState();
        wizardPageState.BeginOperation(new WizardOperation());
        wizardPageState.Changed += _ => ++wizardPageStateChangedCounter;

        static async Task RandomUpdateTask(Random random, WizardPageState wizardPageState)
        {
            wizardPageState.BeginOperation(new WizardOperation());
            try
            {
                var delay = random.Next(0, 100);
                await Task.Delay(delay);
            }
            finally
            {
                wizardPageState.EndOperation();
            }
        }

        // Act
        try
        {
            var random = new Random();

            var updateTasks = new List<Task>();
            for (var i = 0; i < 1000; i++)
                updateTasks.Add(RandomUpdateTask(random, wizardPageState));

            await Task.WhenAll(updateTasks);
        }
        finally
        {
            wizardPageState.EndOperation();
        }

        // Assert
        wizardPageStateChangedCounter.Should().Be(1);
    }
}
