using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Services;
using Xunit;

namespace Sdk.Client.Tests.Wizards.Components;

public sealed class WizardTests
{
    [Fact]
    public async Task Should_render()
    {
        // Arrange
        await using var testContext = SetupTestContext();

        // Act
        var component = testContext.Render<Wizard<WizardContext>>(b =>
        {
            b.Add(p => p.Title, "Test Wizard");
            b.Add(p => p.Context, new WizardContext());
        });

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_pass_parameters_to_wizard_content()
    {
        // Arrange
        await using var testContext = SetupTestContext();

        var context = new WizardContext();
        const string Title = "Test Wizard";
        const bool Visible = true;
        const bool AllowExit = true;

        // Act
        var component = testContext.Render<Wizard<WizardContext>>(b =>
        {
            b.Add(p => p.Title, Title);
            b.Add(p => p.Context, context);
            b.Add(p => p.Visible, Visible);
            b.Add(b => b.AllowExit, AllowExit);
        });

        // Assert
        var wizardContent = component.FindComponent<WizardContent<WizardContext>>().Instance;
        wizardContent.Should().NotBeNull();
        wizardContent.Title.Should().Be(Title);
        wizardContent.Context.Should().Be(context);
        wizardContent.Visible.Should().Be(Visible);
        wizardContent.AllowExit.Should().Be(AllowExit);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Should_raise_visible_changed(bool initialValue, bool value)
    {
        // Arrange
        await using var testContext = SetupTestContext();

        var eventCallbackReceiver = new WizardEventCallbackReceiver { Visible = initialValue };

        var component = testContext.Render<Wizard<WizardContext>>(b =>
        {
            b.Add(p => p.Title, "Test Wizard");
            b.Add(p => p.Context, new WizardContext());
            b.Add(p => p.Visible, initialValue);
            b.Add(p => p.VisibleChanged, EventCallback.Factory.Create<bool>(eventCallbackReceiver, eventCallbackReceiver.UpdateVisible));
        });

        var wizardContent = component.FindComponent<WizardContent<WizardContext>>();

        // Act
        await wizardContent.InvokeAsync(async () => await wizardContent.Instance.RaiseVisibleChanged(value));

        // Assert
        eventCallbackReceiver.Visible.Should().Be(value);
    }

    [Fact]
    public async Task Should_throw_on_wrong_wizard_content_type_1()
    {
        // Arrange
        await using var testContext = SetupTestContext(typeof(IWizardContent<WizardContext>));

        // Act
        Action act = () => testContext.Render<Wizard<WizardContext>>(b =>
        {
            b.Add(p => p.Title, "Test Wizard");
            b.Add(p => p.Context, new WizardContext());
        });

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Component type returned by * is not a concrete class");
    }

    [Fact]
    public async Task Should_throw_on_wrong_wizard_content_type_2()
    {
        // Arrange
        await using var testContext = SetupTestContext(typeof(WizardTests));

        // Act
        Action act = () => testContext.Render<Wizard<WizardContext>>(b =>
        {
            b.Add(p => p.Title, "Test Wizard");
            b.Add(p => p.Context, new WizardContext());
        });

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Component type returned by * does not implement *");
    }

    private static BunitContext SetupTestContext(Type? wizardContentComponentType = null)
    {
        var testContext = new BunitContext();

        testContext.Services.AddScoped<IWizardContentComponentTypeProvider>(
            _ => new WizardContentComponentTypeProvider(wizardContentComponentType));

        return testContext;
    }

    private sealed class WizardContext;

    private sealed class WizardEventCallbackReceiver
    {
        public bool Visible { get; set; }

        public void UpdateVisible(bool value)
            => Visible = value;
    }

    private sealed class WizardContent<TContext> : ComponentBase, IWizardContent<TContext>
    {
        [Parameter, EditorRequired] public required string Title { get; set; }
        [Parameter, EditorRequired] public required TContext Context { get; set; }
        [Parameter] public bool Visible { get; set; }
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
        [Parameter] public bool AllowExit { get; set; }

        public async Task RaiseVisibleChanged(bool value)
        {
            if (VisibleChanged.HasDelegate)
                await VisibleChanged.InvokeAsync(value);
        }
    }

    private sealed class WizardContentComponentTypeProvider(Type? wizardContentComponentType) : IWizardContentComponentTypeProvider
    {
        public Type GetWizardContentComponentType<TContext>()
            => wizardContentComponentType ?? typeof(WizardContent<TContext>);
    }
}
