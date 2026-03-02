using Sdk.Messaging;

namespace Sdk.Client.Infrastructure;

/// <summary>
/// Defines a consumer for a specific type of event within the UI infrastructure.
/// </summary>
public interface IEventConsumer<T> where T : class, IEvent
{
    /// <summary>
    /// Handles an incoming event notification.
    /// </summary>
    Task Consume(ClientContext<T> context, CancellationToken cancellationToken);
}

/// <summary>
/// Encapsulates an event message and its associated metadata for consumption by a <see cref="IEventConsumer{T}"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public record ClientContext<T>(T Message, Guid? CorrelationId) where T : class;
