using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsInformationTests
{
    [Fact]
    public async Task Should_render_child_content()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsInformation>(b =>
            b.Add(p => p.ChildContent, "<span>Info text</span>"));

        // Assert
        component.Find(".settings-information").Should().NotBeNull();
        component.Find(".content").InnerHtml.Should().Contain("Info text");
    }
}
