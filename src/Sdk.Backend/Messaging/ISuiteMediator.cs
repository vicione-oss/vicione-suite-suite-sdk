using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// Sends commands, publishes events and makes requests from backend module code; the message interface decides where a
/// message is routed.
/// </summary>
public interface ISuiteMediator
{
    /// <summary>
    /// Sends a command to the master, which processes it; the result arrives as an event, not as a return value.
    /// </summary>
    Task Send<TCommand>(TCommand message, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    /// <summary>
    /// Sends a command to the instance <paramref name="instanceId"/>, e.g. for a state change outside the module database.
    /// </summary>
    Task Send<TCommand>(TCommand message, Guid instanceId, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand;

    /// <summary>
    /// Publishes an event to every subscriber on every instance.
    /// </summary>
    Task Publish<TEvent>(TEvent message, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;

    /// <summary>
    /// Sends a request that is answered within this instance and awaits the response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>;

    /// <summary>
    /// Sends a request that is answered within this instance and awaits the response for at most <paramref name="timeout"/>.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>;

    /// <summary>
    /// Sends a request to the instance <paramref name="instanceId"/> and awaits its response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;

    /// <summary>
    /// Sends a request to the instance <paramref name="instanceId"/> and awaits its response for at most <paramref name="timeout"/>.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, TimeSpan timeout,
        CancellationToken cancellationToken = default)
            where TRequest : class, IInstanceDependentRequest<TResponse>
            where TResponse : class, IResponse;
}

