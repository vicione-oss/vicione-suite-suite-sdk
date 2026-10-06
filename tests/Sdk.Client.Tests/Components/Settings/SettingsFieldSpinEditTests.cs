using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using ViciOne.Ui.Blazor.Components.SpinEdit;
using ViciOne.Ui.Blazor.Components.SpinEdit.Extensions;
using ViciOne.Ui.Blazor.Components.TestingHelpers.TextBox.Extensions;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldSpinEditTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        component.Find(".settings-field-spin-edit").Should().NotBeNull();
    }

    [Fact]
    public async Task Should_forward_value()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Value, 42);
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.Value.Should().Be(42);
    }

    [Fact]
    public async Task Should_forward_interval()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Interval, 5);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.Interval.Should().Be(5);
    }

    [Fact]
    public async Task Should_forward_minimum()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 10);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.Minimum.Should().Be(10);
    }

    [Fact]
    public async Task Should_forward_maximum()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 50);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.Maximum.Should().Be(50);
    }

    [Fact]
    public async Task Should_forward_is_rastered()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.IsRastered, true);
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.IsRastered.Should().BeTrue();
    }

    [Fact]
    public async Task Should_forward_read_only()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.ReadOnly, true);
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.ReadOnly.Should().BeTrue();
    }

    [Fact]
    public async Task Should_forward_enabled()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Enabled, false);
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.Enabled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_forward_placeholder()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Placeholder, "Enter value");
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.Placeholder.Should().Be("Enter value");
    }

    [Fact]
    public async Task Should_forward_css_class()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        // Act
        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.CssClass, "custom-class");
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Assert
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        spinEdit.Instance.CssClass.Should().Be("custom-class");
    }

    [Fact]
    public async Task Should_forward_value_changed()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();
        var changedValue = 0;

        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.Value, 10);
            b.Add(p => p.ValueChanged, (int v) => changedValue = v);
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        // Act
        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();
        await spinEdit.InvokeAsync(() => spinEdit.Instance.ValueChanged.InvokeAsync(25));

        // Assert
        changedValue.Should().Be(25);
    }

    [Fact]
    public async Task Should_propagate_exception_of_value_changed_handler()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.JSInterop.SetupForTextBox();
        ctx.Services.AddIntSpinEdit();

        var component = ctx.Render<SettingsFieldSpinEdit<int, int, int>>(b =>
        {
            b.Add(p => p.ValueChanged, (int _) => Task.FromException(new InvalidOperationException("Handler failed.")));
            b.Add(p => p.Interval, 1);
            b.Add(p => p.Minimum, 0);
            b.Add(p => p.Maximum, 100);
        });

        var spinEdit = component.FindComponent<SpinEdit<int, int, int>>();

        // Act
        var act = () => spinEdit.InvokeAsync(() => spinEdit.Instance.ValueChanged.InvokeAsync(25));

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Handler failed.");
    }
}
