using AwesomeAssertions;
using Sdk.Client.Colors;
using Xunit;

namespace Sdk.Client.Tests.Colors;

public sealed class StandardHtmlColorTests
{
    [Fact]
    public void From_should_create_color_with_css_custom_property()
    {
        // Act
        var color = StandardHtmlColor.From(StandardColor.Red);

        // Assert
        color.ToString().Should().StartWith("var(--vo-standard-color-");
    }

    [Fact]
    public void ToString_should_return_hyphen_separated_css_variable()
    {
        // Act
        var color = StandardHtmlColor.From(StandardColor.Red);

        // Assert
        color.ToString().Should().Be("var(--vo-standard-color-red)");
    }

    [Theory]
    [InlineData(StandardColor.Red)]
    [InlineData(StandardColor.Green)]
    [InlineData(StandardColor.Blue)]
    [InlineData(StandardColor.Orange)]
    [InlineData(StandardColor.LightGreen)]
    public void From_should_create_valid_color_for_all_standard_colors(StandardColor standardColor)
    {
        // Act
        var color = StandardHtmlColor.From(standardColor);

        // Assert
        color.ToString().Should().StartWith("var(--vo-standard-color-");
        color.ToString().Should().EndWith(")");
    }
}
