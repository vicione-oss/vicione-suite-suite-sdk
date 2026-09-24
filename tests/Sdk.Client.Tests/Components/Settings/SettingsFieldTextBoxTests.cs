using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
using ViciOne.Ui.Blazor.Components.TextBox;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldTextBoxTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldTextBox>();

        // Assert
        component.Find(".settings-field-text-box").Should().NotBeNull();
    }

    [Fact]
    public async Task Should_render_subline_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldTextBox>(b =>
            b.Add(p => p.Subline, "Help text"));

        // Assert
        component.Find(".subline").TextContent.Should().Contain("Help text");
    }

    [Fact]
    public async Task Should_not_render_subline_when_empty()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldTextBox>();

        // Assert
        component.FindAll(".subline").Should().BeEmpty();
    }

    [Fact]
    public async Task Should_propagate_exception_of_value_changed_handler()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        var component = ctx.Render<SettingsFieldTextBox>(b =>
            b.Add(p => p.ValueChanged, (string _) => Task.FromException(new InvalidOperationException("Handler failed."))));

        var textBox = component.FindComponent<TextBox>();

        // Act
        var act = () => textBox.InvokeAsync(() => textBox.Instance.ValueChanged.InvokeAsync("changed"));

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Handler failed.");
    }
}
