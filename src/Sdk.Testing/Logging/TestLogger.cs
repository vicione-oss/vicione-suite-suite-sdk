using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Sdk.Testing.Logging;

/// <summary>
/// Records every entry at or above the given minimum level in <see cref="Entries"/>, and the latest one in the
/// <see cref="LogLevel"/>, <see cref="EventId"/>, <see cref="Exception"/> and <see cref="Message"/> properties.
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
    /// Gets all captured entries in logging order; safe to read while other threads log.
    /// </summary>
    public ConcurrentQueue<TestLogEntry> Entries { get; } = new();

    /// <summary>
    /// Returns a throw-away disposable; scopes and their state are not recorded.
    /// </summary>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => new TestLogger<T>(_logLevel);

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

    /// <inheritdoc/>
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
    /// Does nothing; the logger holds no resources.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// Clears <see cref="Entries"/>; the properties describing the latest entry keep their values.
    /// </summary>
    public void Clear()
        => Entries.Clear();
}

/// <summary>
/// Represents a single log entry captured by the <see cref="TestLogger{T}"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public record TestLogEntry(LogLevel LogLevel, string Message, Exception? Exception = default, EventId EventId = default);
