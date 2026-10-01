using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsLayoutTests
{
    [Fact]
    public async Task Should_render_child_content_within_layout_div()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsLayout>(b =>
            b.Add(p => p.ChildContent, "<div class='inner'>Content</div>"));

        // Assert
        component.Find(".settings-layout").Should().NotBeNull();
        component.Find(".inner").TextContent.Should().Be("Content");
    }

    [Fact]
    public async Task Should_not_set_style_when_content_padding_right_is_null()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsLayout>(b =>
            b.Add(p => p.ChildContent, "Content"));

        // Assert
        component.Find(".settings-layout").HasAttribute("style").Should().BeFalse();
    }

    [Fact]
    public async Task Should_set_padding_custom_property_when_content_padding_right_is_set()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsLayout>(b => b
            .Add(p => p.ChildContent, "Content")
            .Add(p => p.ContentPaddingRight, 16));

        // Assert
        component.Find(".settings-layout").GetAttribute("style").Should().Be("--settings-content-padding-right: 16px;");
    }
}
