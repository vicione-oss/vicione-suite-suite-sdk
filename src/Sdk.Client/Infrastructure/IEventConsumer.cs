using Sdk.Messaging;

namespace Sdk.Client.Infrastructure;

public interface IEventConsumer<T> where T : class, IEvent
{
    /// <summary>Handles an event notification</summary>
    /// <param name="context">Message context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task Consume(ClientContext<T> context, CancellationToken cancellationToken);
}

public record ClientContext<T>(T Message, Guid? CorrelationId) where T : class;
