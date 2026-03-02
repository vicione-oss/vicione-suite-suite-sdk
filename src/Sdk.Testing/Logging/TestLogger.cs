using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Sdk.Testing.Logging;

/// <summary>
/// An in-memory logger implementation for testing purposes.
/// </summary>
public sealed class TestLogger<T>(LogLevel logLevel = LogLevel.Trace) : ILogger<T>, IDisposable
{
    private readonly LogLevel _logLevel = logLevel;

    /// <summary>
    /// Gets the total number of log entries captured.
    /// </summary>
    public int Calls => Entries.Count;

    /// <summary>
    /// Gets the log level of the last captured entry.
    /// </summary>
    public LogLevel LogLevel { get; private set; }

    /// <summary>
    /// Gets the event ID of the last captured entry.
    /// </summary>
    public EventId EventId { get; private set; }

    /// <summary>
    /// Gets the exception associated with the last captured entry, if any.
    /// </summary>
    public Exception? Exception { get; private set; }

    /// <summary>
    /// Gets the formatted message of the last captured entry.
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// Gets the concurrent queue containing all captured log entries.
    /// </summary>
    public ConcurrentQueue<TestLogEntry> Entries { get; } = new();

    /// <summary>
    /// Begins a logical operation scope. This implementation returns a new disposable logger instance.
    /// </summary>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => new TestLogger<T>(_logLevel);

    /// <summary>
    /// Checks if the given <paramref name="logLevel"/> is enabled.
    /// </summary>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

    /// <summary>
    /// Writes a log entry if the specified log level is enabled.
    /// </summary>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            Message = formatter(state, exception);
            Entries.Enqueue(new(logLevel, Message, exception, eventId));
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// Clears all captured log entries.
    /// </summary>
    public void Clear()
        => Entries.Clear();
}

/// <summary>
/// Represents a single log entry captured by the <see cref="TestLogger{T}"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public record TestLogEntry(LogLevel LogLevel, string Message, Exception? Exception = default, EventId EventId = default);
