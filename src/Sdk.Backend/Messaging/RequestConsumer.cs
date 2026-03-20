using System.Diagnostics.CodeAnalysis;
using MassTransit;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// A base consumer for handling request / response messages.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
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
            await context.RespondAsync(await Respond(context.Message, context.CancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await context.RespondAsync(await HandleException(context.Message, e, context.CancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handles an incoming request and produces a successful response.
    /// </summary>
    public abstract Task<TResponse> Respond(TRequest message, CancellationToken cancellationToken);

    /// <summary>
    /// Handles an exception occurred during <see cref="Respond"/>.
    /// </summary>
    public abstract Task<TResponse> HandleException(TRequest message, Exception e, CancellationToken cancellationToken);
}

