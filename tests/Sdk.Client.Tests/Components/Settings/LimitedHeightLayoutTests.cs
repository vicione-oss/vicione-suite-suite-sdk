using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class LimitedHeightLayoutTests
{
    [Fact]
    public async Task Should_render_child_content_within_layout_div()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<LimitedHeightLayout>(b =>
            b.Add(p => p.ChildContent, "<div class='inner'>Content</div>"));

        // Assert
        component.Find(".limited-height-layout .inner").TextContent.Should().Be("Content");
    }
}
