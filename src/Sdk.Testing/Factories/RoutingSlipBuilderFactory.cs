using MassTransit;
using Sdk.Backend.Messaging;

namespace Sdk.Testing.Factories;

internal sealed class RoutingSlipBuilderFactory : IRoutingSlipBuilderFactory
{
    public IRoutingSlipBuilder Create(Guid trackingNumber) => new RoutingSlipBuilder(trackingNumber);
}
