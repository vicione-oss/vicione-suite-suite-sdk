using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Sdk.Testing.Logging;

public sealed class TestLogger<T>(LogLevel logLevel = LogLevel.Trace) : ILogger<T>, IDisposable
{
    private readonly LogLevel _logLevel = logLevel;

    public int Calls => Entries.Count;
    public LogLevel LogLevel { get; private set; }
    public EventId EventId { get; private set; }
    public Exception? Exception { get; private set; }
    public string? Message { get; private set; }
    public ConcurrentQueue<TestLogEntry> Entries { get; } = new();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => new TestLogger<T>(_logLevel);

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

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

    public void Dispose() { }

    public void Clear()
        => Entries.Clear();
}

public record TestLogEntry(LogLevel LogLevel, string Message, Exception? Exception = default, EventId EventId = default);
