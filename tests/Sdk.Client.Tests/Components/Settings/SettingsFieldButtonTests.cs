using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
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
}
