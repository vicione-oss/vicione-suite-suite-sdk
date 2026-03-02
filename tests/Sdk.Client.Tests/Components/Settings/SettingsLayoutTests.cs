using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsLayoutTests
{
    [Fact]
    public async Task Should_render_child_content_within_layout_div()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsLayout>(b =>
            b.Add(p => p.ChildContent, "<div class='inner'>Content</div>"));

        // Assert
        component.Find(".settings-layout").Should().NotBeNull();
        component.Find(".inner").TextContent.Should().Be("Content");
    }
}
