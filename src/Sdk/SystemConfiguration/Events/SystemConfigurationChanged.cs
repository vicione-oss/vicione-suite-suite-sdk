using MassTransit;
using Sdk.Messaging;

namespace Sdk.SystemConfiguration.Events;

[ForwardToUI]
public record SystemConfigurationChanged(Guid CorrelationId) : IEvent, CorrelatedBy<Guid>;
