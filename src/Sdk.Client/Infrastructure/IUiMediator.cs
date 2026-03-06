using Sdk.Messaging;

namespace Sdk.Client.Infrastructure;

/// <summary>
/// Defines a mediator for command, requests and events.
/// </summary>
public interface IUiMediator
{
    /// <summary>
    /// The default command timeout configured for the UI, in milliseconds.
    /// </summary>
    int CommandTimeoutMs { get; }

    /// <summary>
    /// Sends a command for asynchronous processing.
    /// </summary>
    Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    /// <summary>
    /// Sends a command that is dependent on a specific instance for processing.
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
    /// Sends a request that is dependent on a specific instance and awaits a response.
    /// </summary>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;

    /// <summary>
    /// Registers a consumer for a specific type of event.
    /// </summary>
    IDisposable Register<TEvent>(IEventConsumer<TEvent> handler)
        where TEvent : class, IEvent;
}
