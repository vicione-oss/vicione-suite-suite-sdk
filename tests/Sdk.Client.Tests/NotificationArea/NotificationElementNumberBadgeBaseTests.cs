using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed class NotificationElementNumberBadgeBaseTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementNumberBadge>();

        // Assert
        renderedComponent.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_not_be_visible()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementNumberBadge>();

        // Assert
        renderedComponent.Markup.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Should_show_positive_number()
    {
        // Arrange
        await using var ctx = new BunitContext();
        const int Number = 8;

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementNumberBadge>(p => p.Add(c => c.Number, Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be($"{Number}");
    }

    [Fact]
    public async Task Should_show_9_plus()
    {
        // Arrange
        await using var ctx = new BunitContext();
        const int Number = 11;

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementNumberBadge>(p => p.Add(c => c.Number, Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be("9");
        numberBadge.ClassList.Should().Contain("number-badge--plus");
    }

    [Fact]
    public async Task Should_show_negative_number()
    {
        // Arrange
        await using var ctx = new BunitContext();
        const int Number = -7;

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementNumberBadge>(p => p.Add(c => c.Number, Number));

        // Assert
        var numberBadge = renderedComponent.Find(".number-badge");
        numberBadge.TextContent.Should().Be($"{Number}");
    }

    [Fact]
    public async Task Should_show_9_minus()
    {
        // Arrange
        await using var ctx = new BunitContext();
        const int Number = -13;

        // Act
        var renderedComponent = ctx.Render<TestNotificationElementNumberBadge>(p => p.Add(c => c.Number, Number));

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
