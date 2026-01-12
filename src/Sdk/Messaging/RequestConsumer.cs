using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A base consumer for handling request / response messages.
/// </summary>
public abstract class RequestConsumer<TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, IRequest<TResponse>
    where TResponse : IResponse
{
    /// <summary>
    /// Consumes the incoming request message from the bus and responds with either a successful result
    /// or an error response if an exception occurs.
    /// </summary>
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
    protected abstract Task<TResponse> Respond(ConsumeContext<TRequest> context);

    /// <summary>
    /// Handles an exception occurred during <see cref="Respond"/>.
    /// </summary>
    protected abstract Task<TResponse> HandleException(ConsumeContext<TRequest> context, Exception e);
}

