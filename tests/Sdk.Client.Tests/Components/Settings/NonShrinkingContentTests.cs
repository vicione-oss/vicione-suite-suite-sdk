using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class NonShrinkingContentTests
{
    [Fact]
    public async Task Should_render_child_content_within_non_shrinking_div()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<NonShrinkingContent>(b =>
            b.Add(p => p.ChildContent, "<div class='inner'>Content</div>"));

        // Assert
        component.Find(".non-shrinking-content .inner").TextContent.Should().Be("Content");
    }
}
