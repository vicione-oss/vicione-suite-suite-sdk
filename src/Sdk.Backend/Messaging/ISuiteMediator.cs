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
    /// <typeparam name="TCommand">The type of command being sent.</typeparam>
    /// <param name="message">The command message to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task Send<TCommand>(TCommand message, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    /// <summary>
    /// Sends an instance‑specific command message for processing.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being sent, which is instance‑dependent.</typeparam>
    /// <param name="message">The instance‑specific command message to send.</param>
    /// <param name="instanceId">The target instance identifier that this command applies to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task Send<TCommand>(TCommand message, Guid instanceId, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand;

    /// <summary>
    /// Publishes an event message to all interested subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event being published.</typeparam>
    /// <param name="message">The event message to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task Publish<TEvent>(TEvent message, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;

    /// <summary>
    /// Sends a request message and awaits a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of request message being sent.</typeparam>
    /// <typeparam name="TResponse">The expected type of the response message.</typeparam>
    /// <param name="request">The request message to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning the response message.</returns>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>;

    /// <summary>
    /// Sends a request message and awaits a response within a specified timeout.
    /// </summary>
    /// <typeparam name="TRequest">The type of request message being sent.</typeparam>
    /// <typeparam name="TResponse">The expected type of the response message.</typeparam>
    /// <param name="request">The request message to send.</param>
    /// <param name="timeout">The maximum time to wait for a response.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning the response message.</returns>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>;

    /// <summary>
    /// Sends an instance‑specific request message and awaits a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of instance‑specific request message being sent.</typeparam>
    /// <typeparam name="TResponse">The expected type of the response message.</typeparam>
    /// <param name="request">The request message to send.</param>
    /// <param name="instanceId">The target instance identifier that this request applies to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning the response message.</returns>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;

    /// <summary>
    /// Sends an instance‑specific request message and awaits a response within a specified timeout.
    /// </summary>
    /// <typeparam name="TRequest">The type of instance‑specific request message being sent.</typeparam>
    /// <typeparam name="TResponse">The expected type of the response message.</typeparam>
    /// <param name="request">The request message to send.</param>
    /// <param name="instanceId">The target instance identifier that this request applies to.</param>
    /// <param name="timeout">The maximum time to wait for a response.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning the response message.</returns>
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;
}

