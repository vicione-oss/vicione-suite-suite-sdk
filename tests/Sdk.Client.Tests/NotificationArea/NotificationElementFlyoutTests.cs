using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.NotificationArea.Services;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed class NotificationElementFlyoutTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementFlyout>(p => p.AddCascadingValue(state));

        // Assert
        renderedComponent.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_be_visible()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState { IsActive = true };

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementFlyout>(p => p.AddCascadingValue(state));

        // Assert
        var flyout = renderedComponent.Find(".notification-element-flyout");
        flyout.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_render_heading()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState { IsActive = true };
        const string HeadingText = "Test heading";

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementFlyout>(p => p.Add(c => c.Heading, HeadingText)
                                                                                .AddCascadingValue(state));

        // Assert
        var heading = renderedComponent.Find(".heading");
        heading.Should().NotBeNull();
        heading.TextContent.Should().Be(HeadingText);
    }

    [Fact]
    public async Task Should_render_content()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState { IsActive = true };

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementFlyout>(p => p.AddCascadingValue(state));

        // Assert
        var content = renderedComponent.Find(".content");
        content.TextContent.Should().Be(TestNotificationElementFlyoutContent.TextContent);
    }

    private sealed class TestNotificationElementFlyoutContent : ComponentBase, INotificationElementFlyoutContent
    {
        public const string TextContent = nameof(TestNotificationElementFlyoutContent);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.AddContent(0, TextContent);
        }
    }

    private sealed class TestNotificationElementFlyout : NotificationElementFlyout<NotificationElementState, TestNotificationElementFlyoutContent>
    {
        [Parameter] public string? Heading { get; set; }
        [Parameter] public bool? CloseButtonVisible { get; set; }

        protected override string? GetHeading() => Heading;
    }
}
