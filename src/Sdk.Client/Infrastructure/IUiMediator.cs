using Sdk.Messaging;

namespace Sdk.Client.Infrastructure;

/// <summary>
/// Sends commands and requests from UI code and subscribes it to events; the UI counterpart of the backend's mediator.
/// </summary>
public interface IUiMediator
{
    /// <summary>
    /// Gets how long the UI waits for the outcome of a command, in milliseconds.
    /// </summary>
    int CommandTimeoutMs { get; }

    /// <summary>
    /// Sends a command to the master; the outcome arrives as an event, not as a return value.
    /// </summary>
    Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    /// <summary>
    /// Sends a command to the instance <paramref name="instanceId"/>.
    /// </summary>
    Task Send<TCommand>(TCommand command, Guid instanceId, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand;

    /// <summary>
    /// Sends a request and awaits a response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequest<TResponse>
        where TResponse : class, IResponse;

    /// <summary>
    /// Sends a request to the instance <paramref name="instanceId"/> and awaits its response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;

    /// <summary>
    /// Subscribes <paramref name="handler"/> to <typeparamref name="TEvent"/>; dispose the result to unsubscribe.
    /// </summary>
    IDisposable Register<TEvent>(IEventConsumer<TEvent> handler)
        where TEvent : class, IEvent;
}
