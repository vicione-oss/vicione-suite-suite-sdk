using System.Collections.Concurrent;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Sdk.Backend.Messaging;

/// <summary>
/// Base class of a consumer that reacts to the completion or fault of the routing slips it started with <see cref="ExecuteTracked"/>.
/// </summary>
/// <remarks>
/// Tracking lives in memory in this process: a slip started before a restart, or by another process, is ignored.
/// </remarks>
public abstract class TrackingConsumerBase : IConsumer<RoutingSlipCompleted>, IConsumer<RoutingSlipFaulted>
{
    private static readonly ConcurrentDictionary<Guid, Type> s_activeRoutingSlips = new();

    private void AddActivityTracking(Guid trackingNumber)
        => s_activeRoutingSlips.TryAdd(trackingNumber, GetType());

    private bool RemoveActivityTracking(Guid trackingNumber)
        => s_activeRoutingSlips.TryGetValue(trackingNumber, out var consumerType)
           && consumerType == GetType()
           && s_activeRoutingSlips.TryRemove(trackingNumber, out _);

    /// <summary>
    /// Forwards the event to <see cref="ConsumeCompleted"/> if this consumer type started the slip; ignores it otherwise.
    /// </summary>
    public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
    {
        if (!RemoveActivityTracking(context.Message.TrackingNumber))
            return Task.CompletedTask;

        return ConsumeCompleted(context);
    }

    /// <summary>
    /// Builds and executes the routing slip, remembering its <see cref="IItineraryBuilder.TrackingNumber"/> so that its
    /// completion or fault reaches this consumer type.
    /// </summary>
    protected Task ExecuteTracked<T>(T source, IRoutingSlipBuilder builder)
        where T : IPublishEndpoint, ISendEndpointProvider
    {
        var routingSlip = builder.Build();

        AddActivityTracking(builder.TrackingNumber);

        return source.Execute(routingSlip);
    }

    /// <summary>
    /// Handles the completion of a routing slip this consumer type started.
    /// </summary>
    protected abstract Task ConsumeCompleted(ConsumeContext<RoutingSlipCompleted> context);

    /// <summary>
    /// Forwards the event to <see cref="ConsumeFaulted"/> if this consumer type started the slip; ignores it otherwise.
    /// </summary>
    public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
    {
        if (!RemoveActivityTracking(context.Message.TrackingNumber))
            return Task.CompletedTask;

        return ConsumeFaulted(context);
    }

    /// <summary>
    /// Handles the fault of a routing slip this consumer type started.
    /// </summary>
    protected abstract Task ConsumeFaulted(ConsumeContext<RoutingSlipFaulted> context);
}
