using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Sdk.Testing.Logging;
using Xunit;

namespace Sdk.Testing.Tests.Logging;

public sealed partial class TestLoggerTests
{
    [Fact]
    public void Log_should_capture_entry()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>();

        // Act
        LogMessage(logger, LogLevel.Information, "Test message");

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
        LogException(logger, LogLevel.Error, exception, "Error occurred");

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
        LogMessage(logger, LogLevel.Debug, "Debug message");
        LogMessage(logger, LogLevel.Information, "Info message");

        // Assert
        logger.Calls.Should().Be(0);
    }

    [Fact]
    public void Log_should_capture_at_minimum_level()
    {
        // Arrange
        using var logger = new TestLogger<TestLoggerTests>(LogLevel.Warning);

        // Act
        LogMessage(logger, LogLevel.Warning, "Warning message");

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
        LogMessage(logger, LogLevel.Information, "Message 1");
        LogMessage(logger, LogLevel.Information, "Message 2");
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
        LogMessage(logger, LogLevel.Information, "First");
        LogMessage(logger, LogLevel.Warning, "Second");
        LogMessage(logger, LogLevel.Error, "Third");

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

    // SkipEnabledCheck hands every entry to TestLogger.Log, so the tests exercise its own minimum level filter.
    [LoggerMessage(Message = "{Message}", SkipEnabledCheck = true)]
    private static partial void LogMessage(ILogger logger, LogLevel level, string message);

    [LoggerMessage(Message = "{Message}", SkipEnabledCheck = true)]
    private static partial void LogException(ILogger logger, LogLevel level, Exception exception, string message);
}

