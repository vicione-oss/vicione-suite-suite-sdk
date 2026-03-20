using MassTransit;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// A base consumer for handling instance‑dependent request / response messages.
/// </summary>
public abstract class InstanceDependentRequestConsumer<TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, IInstanceDependentRequest<TResponse>
    where TResponse : IResponse
{
    /// <summary>
    /// Consumes and incoming request message from the message bus.
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
