using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A base consumer for handling request / response messages.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the incoming request message, which must implement <see cref="IRequest{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response message returned by this consumer, which must implement <see cref="IResponse"/>.
/// </typeparam>
/// <remarks>
/// This base class streamlines request handling by:
/// <list type="bullet">
/// <item><description>Invoking <see cref="Respond"/> to process the request and send a normal response.</description></item>
/// <item><description>Catching exceptions and invoking <see cref="HandleException"/> to send a failure response.</description></item>
/// </list>
/// Derive from this class and implement the abstract methods to define request processing and error handling.
/// </remarks>
public abstract class RequestConsumer<TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, IRequest<TResponse>
    where TResponse : IResponse
{
    /// <summary>
    /// Consumes the incoming request message from the bus and responds with either a successful result
    /// or an error response if an exception occurs.
    /// </summary>
    /// <param name="context">The message consumption context provided by MassTransit.</param>
    /// <returns>
    /// A task representing the asynchronous consumption and response operation.
    /// </returns>    
    public async Task Consume(ConsumeContext<TRequest> context)
    {
        try
        {
            await context.RespondAsync(await Respond(context)).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await context.RespondAsync(await HandleException(context, e)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handles an incoming request and produces a successful response.
    /// </summary>
    /// <param name="context">The message consumption context containing the request message.</param>
    /// <returns>
    /// A task that returns a <typeparamref name="TResponse"/> representing the normal result of processing the request.
    /// </returns>
    protected abstract Task<TResponse> Respond(ConsumeContext<TRequest> context);

    /// <summary>
    /// Handles an exception occurred during <see cref="Respond"/>.
    /// </summary>
    /// <param name="context">The message consumption context containing the request message.</param>
    /// <param name="e">The exception that occurred while processing the request.</param>
    /// <returns>
    /// A task that returns a <typeparamref name="TResponse"/> representing a failure or error result.
    /// </returns>
    protected abstract Task<TResponse> HandleException(ConsumeContext<TRequest> context, Exception e);
}

