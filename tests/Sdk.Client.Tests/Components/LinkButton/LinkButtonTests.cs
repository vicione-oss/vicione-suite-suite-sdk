using Bunit;
using AwesomeAssertions;
using Sdk.Testing.Client;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using Xunit;

namespace Sdk.Client.Tests.Components.LinkButton;

public sealed class LinkButtonTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink));

        // Assert
        component.Find("button").Should().NotBeNull();
        component.Find("button").ClassList.Should().Contain("link-button");
    }

    [Fact]
    public async Task Should_render_text_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
        {
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.Text, "Click me");
        });

        // Assert
        component.Find(".text").TextContent.Should().Contain("Click me");
    }

    [Fact]
    public async Task Should_not_render_text_div_when_text_is_empty()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink));

        // Assert
        component.FindAll(".text").Should().BeEmpty();
    }

    [Fact]
    public async Task Should_add_disabled_attribute_when_not_enabled()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
        {
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.Enabled, false);
        });

        // Assert
        component.Find("button").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public async Task Should_add_title_attribute_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
        {
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.Title, "Tooltip text");
        });

        // Assert
        component.Find("button").GetAttribute("title").Should().Be("Tooltip text");
    }

    [Fact]
    public async Task Should_add_id_attribute_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
        {
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.Id, "my-button");
        });

        // Assert
        component.Find("button").GetAttribute("id").Should().Be("my-button");
    }

    [Fact]
    public async Task Should_apply_custom_css_class()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
        {
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.CssClass, "custom-class");
        });

        // Assert
        component.Find("button").ClassList.Should().Contain("custom-class");
    }

    [Fact]
    public async Task Should_invoke_on_click_callback()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();
        var clicked = false;

        var component = ctx.Render<Client.Components.LinkButton.LinkButton>(b =>
        {
            b.Add(p => p.IconName, MonochromeIconName.ExternalLink);
            b.Add(p => p.OnClick, (_) => clicked = true);
        });

        // Act
        await component.Find("button").ClickAsync();

        // Assert
        clicked.Should().BeTrue();
    }
}
