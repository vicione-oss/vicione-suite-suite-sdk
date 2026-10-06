using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Strip;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using Xunit;

namespace Sdk.Client.Tests.Components.Strip;

public sealed class StripComponentTests
{
    [Fact]
    public async Task Should_render_component_with_headline_and_subline()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<StripComponent>(b =>
        {
            b.Add(p => p.Headline, "Test Headline");
            b.Add(p => p.Subline, "Test Subline");
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
        });

        // Assert
        component.Find(".headline").TextContent.Should().Be("Test Headline");
        component.Find(".subline").TextContent.Should().Be("Test Subline");
    }

    [Fact]
    public async Task Should_add_clickable_class_when_on_click_is_set()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<StripComponent>(b =>
        {
            b.Add(p => p.Headline, "Clickable");
            b.Add(p => p.Subline, "Sub");
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.OnClick, (_) => { });
        });

        // Assert
        component.Find(".strip-row").ClassList.Should().Contain("clickable");
        component.Find(".strip-row").HasAttribute("tabindex").Should().BeTrue();
    }

    [Fact]
    public async Task Should_not_add_clickable_class_when_on_click_is_not_set()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<StripComponent>(b =>
        {
            b.Add(p => p.Headline, "Not Clickable");
            b.Add(p => p.Subline, "Sub");
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
        });

        // Assert
        component.Find(".strip-row").ClassList.Should().NotContain("clickable");
    }

    [Fact]
    public async Task Should_apply_custom_css_class()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<StripComponent>(b =>
        {
            b.Add(p => p.Headline, "Title");
            b.Add(p => p.Subline, "Sub");
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.CssClass, "my-strip");
        });

        // Assert
        component.Find(".strip-row").ClassList.Should().Contain("my-strip");
    }

    [Fact]
    public async Task Should_invoke_on_click_callback()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var clicked = false;

        var component = ctx.Render<StripComponent>(b =>
        {
            b.Add(p => p.Headline, "Click me");
            b.Add(p => p.Subline, "Sub");
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.OnClick, (_) => clicked = true);
        });

        // Act
        await component.Find(".strip-row").ClickAsync();

        // Assert
        clicked.Should().BeTrue();
    }
}
