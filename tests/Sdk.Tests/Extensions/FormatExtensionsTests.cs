using AwesomeAssertions;
using Sdk.Extensions;

namespace Sdk.Tests.Extensions;

public sealed class FormatExtensionsTests
{
    public sealed class CalculateBytesToMb
    {
        [Theory]
        [InlineData(1, 1_048_576)]
        [InlineData(0, 0)]
        [InlineData(10, 10_485_760)]
        [InlineData(100, 104_857_600)]
        [InlineData(1024, 1_073_741_824)]
        public void Should_return_correct_byte_count(int megabytes, long expectedBytes)
        {
            // Act
#pragma warning disable CS0618 // Covers the method until its removal in the next major version.
            var result = megabytes.CalculateBytesToMb();
#pragma warning restore CS0618

            // Assert
            result.Should().Be(expectedBytes);
        }
    }
}

