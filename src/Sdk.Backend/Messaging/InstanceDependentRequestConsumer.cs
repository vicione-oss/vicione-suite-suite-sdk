using MassTransit;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// Base consumer of an instance-dependent request: it always answers, with the result of <see cref="Respond"/> or, if that throws,
/// of <see cref="HandleException"/>.
/// </summary>
public abstract class InstanceDependentRequestConsumer<TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, IInstanceDependentRequest<TResponse>
    where TResponse : IResponse
{
    /// <summary>
    /// Responds with the result of <see cref="Respond"/>, or of <see cref="HandleException"/> if it throws; only an exception
    /// from <see cref="HandleException"/> itself faults the request.
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
    /// Builds the response for an exception thrown by <see cref="Respond"/>, typically carrying the error information.
    /// </summary>
    public abstract Task<TResponse> HandleException(TRequest message, Exception e, CancellationToken cancellationToken);
}
