using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Sdk.Testing.Logging;
using Xunit;

namespace Sdk.Testing.Tests.Logging;

public sealed class TestLoggerTests
{
    [Fact]
    public void Log_should_capture_entry()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();

        // Act
        logger.LogInformation("Test message");

        // Assert
        logger.Calls.Should().Be(1);
        logger.Message.Should().Be("Test message");
        logger.LogLevel.Should().Be(LogLevel.Information);
    }

    [Fact]
    public void Log_should_capture_exception()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();
        var exception = new InvalidOperationException("test error");

        // Act
        logger.LogError(exception, "Error occurred");

        // Assert
        logger.Exception.Should().BeSameAs(exception);
        logger.LogLevel.Should().Be(LogLevel.Error);
    }

    [Fact]
    public void Log_should_not_capture_below_minimum_level()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>(LogLevel.Warning);

        // Act
        logger.LogDebug("Debug message");
        logger.LogInformation("Info message");

        // Assert
        logger.Calls.Should().Be(0);
    }

    [Fact]
    public void Log_should_capture_at_minimum_level()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>(LogLevel.Warning);

        // Act
        logger.LogWarning("Warning message");

        // Assert
        logger.Calls.Should().Be(1);
    }

    [Fact]
    public void IsEnabled_should_respect_minimum_level()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>(LogLevel.Error);

        // Assert
        logger.IsEnabled(LogLevel.Trace).Should().BeFalse();
        logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
        logger.IsEnabled(LogLevel.Information).Should().BeFalse();
        logger.IsEnabled(LogLevel.Warning).Should().BeFalse();
        logger.IsEnabled(LogLevel.Error).Should().BeTrue();
        logger.IsEnabled(LogLevel.Critical).Should().BeTrue();
    }

    [Fact]
    public void Clear_should_remove_all_entries()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();
        logger.LogInformation("Message 1");
        logger.LogInformation("Message 2");
        logger.Calls.Should().Be(2);

        // Act
        logger.Clear();

        // Assert
        logger.Calls.Should().Be(0);
        logger.Entries.Should().BeEmpty();
    }

    [Fact]
    public void BeginScope_should_return_disposable()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();

        // Act
        var scope = logger.BeginScope("test scope");

        // Assert
        scope.Should().NotBeNull();
        scope.Dispose(); // should not throw
    }

    [Fact]
    public void Log_should_store_entries_in_queue()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();

        // Act
        logger.LogInformation("First");
        logger.LogWarning("Second");
        logger.LogError("Third");

        // Assert
        logger.Entries.Should().HaveCount(3);
        logger.Entries.Select(e => e.Message).Should().ContainInOrder("First", "Second", "Third");
    }

    [Fact]
    public void Log_should_capture_event_id()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();
        var eventId = new EventId(42, "TestEvent");

        // Act
        logger.Log(LogLevel.Information, eventId, "Test", null, (s, _) => s);

        // Assert
        logger.EventId.Should().Be(eventId);
    }
}

