using AwesomeAssertions;
using Sdk.Testing.Extensions;
using Xunit;

namespace Sdk.Testing.Tests.Extensions;

public sealed class RandomExtensionsTests
{
    private enum TestEnum
    {
        First,
        Second,
        Third
    }

    [Fact]
    public void NextEnum_should_return_valid_enum_value()
    {
        // Arrange
        var random = new Random(42);

        // Act
        var result = random.NextEnum<TestEnum>();

        // Assert
        Enum.IsDefined(result).Should().BeTrue();
    }

    [Fact]
    public void NextEnum_should_return_different_values_over_multiple_calls()
    {
        // Arrange
        var random = new Random(42);
        var results = new HashSet<TestEnum>();

        // Act
        for (var i = 0; i < 100; i++)
            results.Add(random.NextEnum<TestEnum>());

        // Assert - 100 calls over 3 values hit each of them
        results.Should().HaveCount(3);
    }
}

