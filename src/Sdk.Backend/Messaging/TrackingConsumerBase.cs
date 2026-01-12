using System.Collections.Concurrent;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Sdk.Backend.Messaging;

public abstract class TrackingConsumerBase :
    IConsumer<RoutingSlipCompleted>,
    IConsumer<RoutingSlipFaulted>
{
    private static readonly ConcurrentDictionary<Guid, Type> s_activeRoutingSlips = new();

    private void AddActivityTracking(Guid trackingNumber)
        => s_activeRoutingSlips.TryAdd(trackingNumber, GetType());

    private bool RemoveActivityTracking(Guid trackingNumber)
        => s_activeRoutingSlips.TryGetValue(trackingNumber, out var consumerType)
           && consumerType == GetType()
           && s_activeRoutingSlips.TryRemove(trackingNumber, out _);

    public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
    {
        if (!RemoveActivityTracking(context.Message.TrackingNumber))
            return Task.CompletedTask;

        return ConsumeCompleted(context);
    }

    protected Task ExecuteTracked<T>(T source, IRoutingSlipBuilder builder)
        where T : IPublishEndpoint, ISendEndpointProvider
    {
        var routingSlip = builder.Build();

        AddActivityTracking(builder.TrackingNumber);

        return source.Execute(routingSlip);
    }

    /// <summary>
    /// RoutingSlipCompleted for inheritors where TrackingNumber matches the formerly added tracking number
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract Task ConsumeCompleted(ConsumeContext<RoutingSlipCompleted> context);

    public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
    {
        if (!RemoveActivityTracking(context.Message.TrackingNumber))
            return Task.CompletedTask;

        return ConsumeFaulted(context);
    }

    /// <summary>
    /// RoutingSlipFaulted for inheritors where TrackingNumber matches the formerly added tracking number
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract Task ConsumeFaulted(ConsumeContext<RoutingSlipFaulted> context);
}
