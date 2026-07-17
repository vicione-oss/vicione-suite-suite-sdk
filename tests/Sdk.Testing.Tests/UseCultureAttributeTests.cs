using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace Sdk.Testing.Tests;

public sealed class UseCultureAttributeTests
{
    [Fact]
    [UseCulture("de-DE")]
    public void Should_set_culture_to_german()
    {
        // Assert
        CultureInfo.CurrentCulture.Name.Should().Be("de-DE");
        CultureInfo.CurrentUICulture.Name.Should().Be("de-DE");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void Should_set_culture_to_french() =>
        // Assert
        CultureInfo.CurrentCulture.Name.Should().Be("fr-FR");

    [Fact]
    [UseCulture("en-US", "de-DE")]
    public void Should_set_different_culture_and_ui_culture()
    {
        // Assert
        CultureInfo.CurrentCulture.Name.Should().Be("en-US");
        CultureInfo.CurrentUICulture.Name.Should().Be("de-DE");
    }

    [Fact]
    public void Culture_property_should_return_culture_info()
    {
        // Arrange
        var attribute = new UseCultureAttribute("ja-JP");

        // Assert
        attribute.Culture.Name.Should().Be("ja-JP");
        attribute.UiCulture.Name.Should().Be("ja-JP");
    }

    [Fact]
    public void Culture_property_should_return_separate_ui_culture()
    {
        // Arrange
        var attribute = new UseCultureAttribute("en-GB", "fr-FR");

        // Assert
        attribute.Culture.Name.Should().Be("en-GB");
        attribute.UiCulture.Name.Should().Be("fr-FR");
    }
}

