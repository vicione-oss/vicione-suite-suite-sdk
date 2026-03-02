using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using NSubstitute;
using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Services;
using Xunit;

namespace Sdk.Client.Tests.Wizards.Components;

public sealed class WizardPageTests
{
    [Fact]
    public async Task Should_render()
    {
        // Arrange
        await using var testContext = new BunitContext();

        var state = new WizardPageState();

        // Act
        var component = testContext.Render<WizardPage<WizardPageState>>(b => b.Add(p => p.State, state));

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_raise_on_begin_edit()
    {
        // Arrange
        await using var testContext = new BunitContext();

        var state = new WizardPageState();
        var eventCallbackReceiver = Substitute.For<IWizardPageEventCallbackReceiver>();

        var component = testContext.Render<TestWizardPage>(b =>
        {
            b.Add(p => p.State, state);
            b.Add(p => p.OnBeginEdit, EventCallback.Factory.Create(eventCallbackReceiver, eventCallbackReceiver.BeginEdit));
        });

        // Act
        await component.InvokeAsync(component.Instance.InvokeBeginEdit);

        // Assert
        eventCallbackReceiver.Received().BeginEdit();
    }

    [Fact]
    public async Task Should_raise_on_cancel_edit()
    {
        // Arrange
        await using var testContext = new BunitContext();

        var state = new WizardPageState();
        var eventCallbackReceiver = Substitute.For<IWizardPageEventCallbackReceiver>();

        var component = testContext.Render<TestWizardPage>(b =>
        {
            b.Add(p => p.State, state);
            b.Add(p => p.OnCancelEdit, EventCallback.Factory.Create(eventCallbackReceiver, eventCallbackReceiver.CancelEdit));
        });

        // Act
        await component.InvokeAsync(component.Instance.InvokeCancelEdit);

        // Assert
        eventCallbackReceiver.Received().CancelEdit();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_raise_on_after_render_cycle(bool firstRender)
    {
        // Arrange
        await using var testContext = new BunitContext();

        var state = new WizardPageState();
        var eventCallbackReceiver = Substitute.For<IWizardPageEventCallbackReceiver>();

        // Act
        var component = testContext.Render<TestWizardPage>(b =>
        {
            b.Add(p => p.State, state);

            if (firstRender)
            {
                b.Add(p => p.OnAfterRenderCycle, EventCallback.Factory.Create<WizardPageAfterRenderCycleEventArgs>(
                    eventCallbackReceiver, eventCallbackReceiver.AfterRenderCycle));
            }
        });

        if (!firstRender)
        {
            component.Render(b =>
            {
                b.Add(p => p.OnAfterRenderCycle, EventCallback.Factory.Create<WizardPageAfterRenderCycleEventArgs>(
                    eventCallbackReceiver, eventCallbackReceiver.AfterRenderCycle));
            });
        }

        // Assert
        eventCallbackReceiver.Received().AfterRenderCycle(
            Arg.Do<WizardPageAfterRenderCycleEventArgs>(args => args.FirstRender.Should().Be(firstRender)));
    }

    private sealed class TestWizardPage : WizardPage<WizardPageState>
    {
        public Task InvokeBeginEdit()
            => BeginEdit();

        public Task InvokeCancelEdit()
            => CancelEdit();
    }

    public interface IWizardPageEventCallbackReceiver
    {
        void BeginEdit();
        void CancelEdit();
        void AfterRenderCycle(WizardPageAfterRenderCycleEventArgs args);
    }
}
