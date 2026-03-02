using AwesomeAssertions;
using Sdk.Client.Extensions;
using Xunit;

namespace Sdk.Client.Tests.Extensions;

public sealed class StringExtensionsTests
{
    public sealed class ToHyphenSeparated
    {
        [Theory]
        [InlineData("PascalCase", "pascal-case")]
        [InlineData("MyComponentName", "my-component-name")]
        [InlineData("ABC", "a-b-c")]
        [InlineData("HTMLParser", "h-t-m-l-parser")]
        [InlineData("A", "a")]
        [InlineData("a", "a")]
        [InlineData("already-hyphenated", "already-hyphenated")]
        public void Should_convert_to_hyphen_separated(string input, string expected)
        {
            // Act
            var result = input.ToHyphenSeparated();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Should_return_empty_for_empty_string()
        {
            // Act
            var result = string.Empty.ToHyphenSeparated();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_return_null_for_null_string()
        {
            // Act
            var result = ((string)null!).ToHyphenSeparated();

            // Assert
            result.Should().BeNull();
        }
    }
}

