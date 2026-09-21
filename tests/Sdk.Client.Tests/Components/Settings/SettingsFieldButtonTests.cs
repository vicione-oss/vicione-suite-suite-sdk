using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
using ViciOne.Ui.Blazor.Components.Button.Enums;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldButtonTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldButton>(b =>
            b.Add(p => p.Text, "Save"));

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_invoke_on_click_callback()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();
        var clicked = false;

        var component = ctx.Render<SettingsFieldButton>(b =>
        {
            b.Add(p => p.Text, "Click");
            b.Add(p => p.OnClick, () => clicked = true);
        });

        // Act
        await component.Find("button").ClickAsync();

        // Assert
        clicked.Should().BeTrue();
    }

    [Fact]
    public async Task Should_render_the_button_as_busy()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldButton>(b =>
        {
            b.Add(p => p.Text, "Test");
            b.Add(p => p.Busy, true);
        });

        // Assert
        component.Find("button").GetAttribute("aria-busy").Should().Be("true");
    }

    [Fact]
    public async Task Should_render_the_selected_busy_indication()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldButton>(b =>
        {
            b.Add(p => p.Text, "Test");
            b.Add(p => p.IconCssClass, "monochrome-icon-refresh");
            b.Add(p => p.Busy, true);
            b.Add(p => p.BusyIndication, ButtonBusyIndication.SpinningIcon);
        });

        // Assert
        component.Find("button").ClassList.Should().Contain("button--busy-spinning-icon");
    }
}
