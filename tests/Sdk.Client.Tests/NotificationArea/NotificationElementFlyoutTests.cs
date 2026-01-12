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
    public void Should_render_component()
    {
        // Arrange
        using var ctx = new TestContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementFlyout>(
            ComponentParameter.CreateCascadingValue(null, state));

        // Assert
        renderedComponent.Should().NotBeNull();
    }

    [Fact]
    public void Should_be_visible()
    {
        // Arrange
        using var ctx = new TestContext();
        var state = new NotificationElementState { IsActive = true };

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementFlyout>(
            ComponentParameter.CreateCascadingValue(null, state));

        // Assert
        var flyout = renderedComponent.Find(".notification-element-flyout");
        flyout.Should().NotBeNull();
    }

    [Fact]
    public void Should_render_heading()
    {
        // Arrange
        using var ctx = new TestContext();
        var state = new NotificationElementState { IsActive = true };
        const string HeadingText = "Test heading";

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementFlyout>(
            ComponentParameter.CreateCascadingValue(null, state),
            ComponentParameter.CreateParameter(nameof(TestNotificationElementFlyout.Heading), HeadingText));

        // Assert
        var heading = renderedComponent.Find(".heading");
        heading.Should().NotBeNull();
        heading.TextContent.Should().Be(HeadingText);
    }

    [Fact]
    public void Should_render_content()
    {
        // Arrange
        using var ctx = new TestContext();
        var state = new NotificationElementState { IsActive = true };

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementFlyout>(
            ComponentParameter.CreateCascadingValue(null, state));

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
