using MassTransit;
using Sdk.Messaging;
using Sdk.SystemConfiguration.Contracts.Service;

namespace Sdk.SystemConfiguration;

[ForwardToUI]
public record ControlServiceCompleted(Guid CorrelationId, string ServiceName, ServiceState ServiceState) : IEvent, CorrelatedBy<Guid>;
