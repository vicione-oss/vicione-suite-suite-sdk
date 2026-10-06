using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldTests
{
    [Fact]
    public async Task Should_render_child_content()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsField>(b =>
            b.Add(p => p.ChildContent, "<span>Test Content</span>"));

        // Assert
        component.Find(".settings-field").Should().NotBeNull();
        component.Find(".content-container").InnerHtml.Should().Contain("Test Content");
    }

    [Fact]
    public async Task Should_render_title_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsField>(b =>
        {
            b.Add(p => p.Title, "Field Title");
            b.Add(p => p.ChildContent, "<span>Content</span>");
        });

        // Assert
        component.Find(".title").TextContent.Should().Contain("Field Title");
    }

    [Fact]
    public async Task Should_not_render_title_when_empty()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsField>(b =>
            b.Add(p => p.ChildContent, "<span>Content</span>"));

        // Assert
        component.FindAll(".title").Should().BeEmpty();
    }

    [Fact]
    public async Task Should_render_label_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsField>(b =>
        {
            b.Add(p => p.Label, "My Label");
            b.Add(p => p.ChildContent, "<span>Content</span>");
        });

        // Assert
        component.Find(".label").TextContent.Should().Contain("My Label");
    }

    [Fact]
    public async Task Should_not_render_label_when_empty()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsField>(b =>
            b.Add(p => p.ChildContent, "<span>Content</span>"));

        // Assert
        component.FindAll(".label").Should().BeEmpty();
    }
}
