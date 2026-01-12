using MassTransit;

namespace Sdk.Backend.Messaging;

/// <summary>
/// Defines a factory for creating <see cref="IRoutingSlipBuilder"/> instances.
/// </summary>
public interface IRoutingSlipBuilderFactory
{
    /// <summary>
    /// Creates a new <see cref="IRoutingSlipBuilder"/> with the specified tracking number.
    /// </summary>
    IRoutingSlipBuilder Create(Guid trackingNumber);
}
