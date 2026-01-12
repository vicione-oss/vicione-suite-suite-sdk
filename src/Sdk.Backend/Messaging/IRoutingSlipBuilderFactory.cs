using MassTransit;

namespace Sdk.Backend.Messaging;

public interface IRoutingSlipBuilderFactory
{
    IRoutingSlipBuilder Create(Guid trackingNumber);
}
