using AwesomeAssertions;
using NSubstitute;
using Sdk.Utils;

namespace Sdk.Tests.Utils;

public sealed class AutoDisposeListTests
{
    [Fact]
    public void Dispose_should_dispose_all_items()
    {
        // Arrange
        var item1 = Substitute.For<IDisposable>();
        var item2 = Substitute.For<IDisposable>();
        var item3 = Substitute.For<IDisposable>();

        using var list = new AutoDisposeList<IDisposable> { item1, item2, item3 };

        // Act
        list.Dispose();

        // Assert
        item1.Received(1).Dispose();
        item2.Received(1).Dispose();
        item3.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_should_handle_empty_list()
    {
        // Arrange
        using var list = new AutoDisposeList<IDisposable>();

        // Act & Assert - should not throw
        var act = list.Dispose;
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_support_list_operations()
    {
        // Arrange
        var item = Substitute.For<IDisposable>();
        using var list = new AutoDisposeList<IDisposable> { item };

        // Assert
        list.Should().HaveCount(1);
        list.Should().Contain(item);
    }
}
