using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
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
}
