using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsGroupTests
{
    [Fact]
    public async Task Should_render_with_title()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Group Title");
            b.Add(p => p.Subline, null);
        });

        // Assert
        component.Find(".settings-group").Should().NotBeNull();
        component.Find(".title").TextContent.Should().Contain("Group Title");
    }

    [Fact]
    public async Task Should_render_subline_when_provided()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, "Some description");
        });

        // Assert
        component.Find(".subline").TextContent.Should().Contain("Some description");
    }

    [Fact]
    public async Task Should_not_render_subline_when_null()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
        });

        // Assert
        component.FindAll(".subline").Should().BeEmpty();
    }

    [Fact]
    public async Task Should_add_collapsed_class_when_expanded_is_false()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
            b.Add(p => p.Expanded, false);
        });

        // Assert
        component.Find(".settings-group").ClassList.Should().Contain("settings-group--collapsed");
    }

    [Fact]
    public async Task Should_not_add_collapsed_class_when_expanded_is_true()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
            b.Add(p => p.Expanded, true);
        });

        // Assert
        component.Find(".settings-group").ClassList.Should().NotContain("settings-group--collapsed");
    }

    [Fact]
    public async Task Should_render_default_expander_when_expanded_has_value()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
            b.Add(p => p.Expanded, true);
        });

        // Assert
        component.Find(".expander-container").Should().NotBeNull();
        component.Find(".default-expander").Should().NotBeNull();
    }

    [Fact]
    public async Task Should_not_render_expander_when_expanded_is_null()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
        });

        // Assert
        component.FindAll(".expander-container").Should().BeEmpty();
    }

    [Fact]
    public async Task Should_add_without_child_content_class_when_no_child_content()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
        });

        // Assert
        component.Find(".settings-group").ClassList.Should().Contain("settings-group--without-child-content");
    }

    [Fact]
    public async Task Should_render_child_content()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
            b.Add(p => p.ChildContent, "<div class='test-child'>Child</div>");
        });

        // Assert
        component.Find(".settings-group").ClassList.Should().NotContain("settings-group--without-child-content");
        component.Find(".test-child").TextContent.Should().Be("Child");
    }

    [Fact]
    public async Task Should_invoke_expanded_changed_on_expander_click()
    {
        // Arrange
        await using var ctx = new BunitContext();
        bool? newExpandedValue = null;

        var component = ctx.Render<SettingsGroup>(b =>
        {
            b.Add(p => p.Title, "Title");
            b.Add(p => p.Subline, null);
            b.Add(p => p.Expanded, true);
            b.Add(p => p.ExpandedChanged, val => newExpandedValue = val);
        });

        // Act
        await component.Find(".default-expander").ClickAsync();

        // Assert - clicking when expanded=true should invoke with false
        newExpandedValue.Should().Be(false);
    }
}
