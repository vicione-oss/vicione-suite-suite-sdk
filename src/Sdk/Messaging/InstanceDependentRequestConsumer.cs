using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A base consumer for handling instance‑dependent request / response messages.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the incoming request message, which must implement <see cref="IInstanceDependentRequest{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response message returned by this consumer, which must implement <see cref="IResponse"/>.
/// </typeparam>
/// <remarks>
/// This base class simplifies request handling by:
/// <list type="bullet">
/// <item><description>Automatically responding with the result of <see cref="Respond"/> on success.</description></item>
/// <item><description>Automatically responding with the result of <see cref="HandleException"/> when an exception is thrown.</description></item>
/// </list>
/// Derive from this class and implement the abstract methods to define how requests are handled and how errors are mapped to responses.
/// </remarks>
public abstract class InstanceDependentRequestConsumer<TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, IInstanceDependentRequest<TResponse>
    where TResponse : IResponse
{
    /// <summary>
    /// Consumes and incoming request message from the message bus.
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
    /// A task that returns a <typeparamref name="TResponse"/> containing the normal result of processing the request.
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
