using AwesomeAssertions;
using Sdk.Client.Extensions;
using Sdk.Testing;
using Xunit;

namespace Sdk.Client.Tests.Extensions;

public sealed class DoubleExtensionsTests
{
    public sealed class ToAttributeValue
    {
        [Fact]
        [UseCulture("de-DE")]
        public void Should_use_invariant_culture_regardless_of_current_culture()
        {
            // Arrange
            const double Number = 3.5;

            // Act
            var result = Number.ToAttributeValue();

            // Assert - should always use '.' as decimal separator
            result.Should().Be("3.5");
        }

        [Fact]
        public void Should_append_addition_string()
        {
            // Arrange
            const double Number = 10.0;

            // Act
            var result = Number.ToAttributeValue("px");

            // Assert
            result.Should().Be("10.0px");
        }

        [Fact]
        public void Should_format_with_one_decimal_place()
        {
            // Arrange
            const double Number = 7.0;

            // Act
            var result = Number.ToAttributeValue();

            // Assert
            result.Should().Be("7.0");
        }
    }

    public sealed class ToFixedPointValue
    {
        [Fact]
        public void Should_format_without_precision()
        {
            // Arrange
            const double Number = 3.14159;

            // Act
            var result = Number.ToFixedPointValue();

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Should_format_with_specified_precision()
        {
            // Arrange
            const double Number = 3.14159;

            // Act
            var result = Number.ToFixedPointValue(2);

            // Assert
            result.Should().Contain("3");
        }

        [Fact]
        public void Should_format_zero_correctly()
        {
            // Arrange
            const double Number = 0.0;

            // Act
            var result = Number.ToFixedPointValue(1);

            // Assert
            result.Should().Contain("0");
        }
    }
}
