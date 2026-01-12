using System.Collections.Concurrent;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Sdk.Backend.Messaging;

/// <summary>
/// An abstract base class for consumers that need to track the completion or faulting of a specific routing slip.
/// </summary>
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
    /// Consumes the <see cref="RoutingSlipCompleted"/> event,
    /// forwarding it to the <see cref="ConsumeCompleted"/> method if the tracking number matches.
    /// </summary>
    public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
    {
        if (!RemoveActivityTracking(context.Message.TrackingNumber))
            return Task.CompletedTask;

        return ConsumeCompleted(context);
    }

    /// <summary>
    /// Executes a routing slip and tracks its <see cref="IItineraryBuilder.TrackingNumber"/> for future correlation.
    /// </summary>
    protected Task ExecuteTracked<T>(T source, IRoutingSlipBuilder builder)
        where T : IPublishEndpoint, ISendEndpointProvider
    {
        var routingSlip = builder.Build();

        AddActivityTracking(builder.TrackingNumber);

        return source.Execute(routingSlip);
    }

    /// <summary>
    /// When overridden in a derived class, handles the <see cref="RoutingSlipCompleted"/> event for a tracked routing slip.
    /// </summary>
    protected abstract Task ConsumeCompleted(ConsumeContext<RoutingSlipCompleted> context);

    /// <summary>
    /// Consumes the <see cref="RoutingSlipFaulted"/> event,
    /// forwarding it to the <see cref="ConsumeFaulted"/> method if the tracking number matches.
    /// </summary>
    public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
    {
        if (!RemoveActivityTracking(context.Message.TrackingNumber))
            return Task.CompletedTask;

        return ConsumeFaulted(context);
    }

    /// <summary>
    /// When overridden in a derived class, handles the <see cref="RoutingSlipFaulted"/> event for a tracked routing slip.
    /// </summary>
    protected abstract Task ConsumeFaulted(ConsumeContext<RoutingSlipFaulted> context);
}
