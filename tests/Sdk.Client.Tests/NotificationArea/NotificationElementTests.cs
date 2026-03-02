using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Tests.NotificationArea.Extensions;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed class NotificationElementTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        renderedComponent.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_have_button()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        var button = renderedComponent.Find("button");
        button.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_render_title_as_button_tooltip()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        var button = renderedComponent.Find("button");
        button.GetAttribute("title").Should().Be(TestNotificationElement.Title);
    }

    [Fact]
    public async Task Should_render_as_active()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState { IsActive = true };

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        renderedComponent.Should().BeActive();
    }

    [Fact]
    public async Task Should_render_icon()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        var iconContainer = renderedComponent.Find(".icon-container");
        iconContainer.Should().NotBeNull();
        iconContainer.TextContent.Should().Be(TestNotificationElementIcon.TextContent);
    }

    [Fact]
    public async Task Should_render_badge()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        var badgeContainer = renderedComponent.Find(".badge-container");
        badgeContainer.Should().NotBeNull();
        badgeContainer.TextContent.Should().Be(TestNotificationElementBadge.TextContent);
    }

    [Fact]
    public async Task Should_render_flyout()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        // Assert
        var flyoutContainer = renderedComponent.Find(".flyout-container");
        flyoutContainer.Should().NotBeNull();
        flyoutContainer.TextContent.Should().Be(TestNotificationElementFlyout.TextContent);
    }

    [Fact]
    public async Task Should_be_active_after_click()
    {
        // Arrange
        await using var ctx = new BunitContext();
        var state = new NotificationElementState();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElement>(p => p.Add(c => c.State, state));

        var button = renderedComponent.Find("button");
        await button.ClickAsync();

        renderedComponent.Render();

        // Assert
        renderedComponent.Should().BeActive();
    }

    private sealed class TestNotificationElementIcon : ComponentBase
    {
        public const string TextContent = nameof(TestNotificationElementIcon);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.AddContent(0, TextContent);
        }
    }

    private sealed class TestNotificationElement : NotificationElement<NotificationElementState, TestNotificationElementIcon>
    {
        public const string Title = "Test notification element";

        public TestNotificationElement()
        {
            RegisterBadge<TestNotificationElementBadge>();
            RegisterFlyout<TestNotificationElementFlyout>();
        }

        protected override string GetTitle() => Title;
    }

    private sealed class TestNotificationElementBadge : ComponentBase, INotificationElementBadge
    {
        public const string TextContent = nameof(TestNotificationElementBadge);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.AddContent(0, TextContent);
        }
    }

    private sealed class TestNotificationElementFlyout : ComponentBase, INotificationElementFlyout
    {
        public const string TextContent = nameof(TestNotificationElementFlyout);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.AddContent(0, TextContent);
        }
    }
}
