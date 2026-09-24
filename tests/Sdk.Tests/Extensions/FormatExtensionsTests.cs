using AwesomeAssertions;
using Sdk.Extensions;

namespace Sdk.Tests.Extensions;

public sealed class FormatExtensionsTests
{
    // Covers the method until its removal in the next major version.
#pragma warning disable CS0618 // Type or member is obsolete
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
            var result = megabytes.CalculateBytesToMb();

            // Assert
            result.Should().Be(expectedBytes);
        }
    }
#pragma warning restore CS0618
}

