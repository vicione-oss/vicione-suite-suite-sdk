using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// Defines a mediator abstraction used for sending commands, publishing events, and performing request/response interactions
/// within the ViciOne.Suite infrastructure.
/// </summary>
public interface ISuiteMediator
{
    /// <summary>
    /// Sends a command message for processing.
    /// </summary>
    Task Send<TCommand>(TCommand message, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    /// <summary>
    /// Sends an instance‑specific command message for processing.
    /// </summary>
    Task Send<TCommand>(TCommand message, Guid instanceId, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand;

    /// <summary>
    /// Publishes an event message to all interested subscribers.
    /// </summary>
    Task Publish<TEvent>(TEvent message, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;

    /// <summary>
    /// Sends a request message and awaits a response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>;

    /// <summary>
    /// Sends a request message and awaits a response within a specified timeout.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>;

    /// <summary>
    /// Sends an instance‑specific request message and awaits a response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;

    /// <summary>
    /// Sends an instance‑specific request message and awaits a response within a specified timeout.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, TimeSpan timeout,
        CancellationToken cancellationToken = default)
            where TRequest : class, IInstanceDependentRequest<TResponse>
            where TResponse : class, IResponse;
}

