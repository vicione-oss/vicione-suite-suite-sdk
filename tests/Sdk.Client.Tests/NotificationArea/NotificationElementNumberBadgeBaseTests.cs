using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed class NotificationElementNumberBadgeBaseTests
{
    [Fact]
    public void Should_render_component()
    {
        // Arrange
        using var ctx = new TestContext();

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementNumberBadge>();

        // Assert
        renderedComponent.Should().NotBeNull();
    }

    [Fact]
    public void Should_not_be_visible()
    {
        // Arrange
        using var ctx = new TestContext();

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementNumberBadge>();

        // Assert
        renderedComponent.Markup.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Should_show_positive_number()
    {
        // Arrange
        using var ctx = new TestContext();
        const int Number = 8;

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementNumberBadge>(
            ComponentParameter.CreateParameter(nameof(TestNotificationElementNumberBadge.Number), Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be($"{Number}");
    }

    [Fact]
    public void Should_show_9_plus()
    {
        // Arrange
        using var ctx = new TestContext();
        const int Number = 11;

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementNumberBadge>(
            ComponentParameter.CreateParameter(nameof(TestNotificationElementNumberBadge.Number), Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be("9");
        numberBadge.ClassList.Should().Contain("number-badge--plus");
    }

    [Fact]
    public void Should_show_negative_number()
    {
        // Arrange
        using var ctx = new TestContext();
        const int Number = -7;

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementNumberBadge>(
            ComponentParameter.CreateParameter(nameof(TestNotificationElementNumberBadge.Number), Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be($"{Number}");
    }

    [Fact]
    public void Should_show_9_minus()
    {
        // Arrange
        using var ctx = new TestContext();
        const int Number = -13;

        // Act
        var renderedComponent = ctx.RenderComponent<TestNotificationElementNumberBadge>(
            ComponentParameter.CreateParameter(nameof(TestNotificationElementNumberBadge.Number), Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be("9");
        numberBadge.ClassList.Should().Contain("number-badge--minus");
    }

    private sealed class TestNotificationElementNumberBadge : NotificationElementNumberBadgeBase
    {
        [Parameter] public int? Number { get; set; }

        protected override int? GetNumber() => Number;
    }
}
