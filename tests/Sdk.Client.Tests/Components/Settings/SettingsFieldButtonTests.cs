using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using ViciOne.Ui.Blazor.Components.Button;
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
        var clicked = false;

        var component = ctx.Render<SettingsFieldButton>(b =>
        {
            b.Add(p => p.Text, "Click");
            b.Add(p => p.OnClick, () => clicked = true);
        });
        var button = component.FindComponent<Button>();

        // Act
        await button.InvokeAsync(() => button.Instance.OnClick.InvokeAsync());

        // Assert
        clicked.Should().BeTrue();
    }

    [Fact]
    public async Task Should_forward_busy()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsFieldButton>(b =>
        {
            b.Add(p => p.Text, "Test");
            b.Add(p => p.Busy, true);
        });

        // Assert
        component.FindComponent<Button>().Instance.Busy.Should().BeTrue();
    }

    [Fact]
    public async Task Should_forward_busy_indication()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsFieldButton>(b =>
        {
            b.Add(p => p.Text, "Test");
            b.Add(p => p.BusyIndication, ButtonBusyIndication.SpinningIcon);
        });

        // Assert
        component.FindComponent<Button>().Instance.BusyIndication.Should().Be(ButtonBusyIndication.SpinningIcon);
    }
}
