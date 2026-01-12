using MassTransit;
using Sdk.Messaging;
using Sdk.SystemConfiguration.Contracts.Service;

namespace Sdk.SystemConfiguration;

/// <summary>
/// Represents an event indicating that a <see cref="ControlService"/> command has completed.
/// </summary>
[ForwardToUI]
public record ControlServiceCompleted(Guid CorrelationId, string ServiceName, ServiceState ServiceState) : IEvent, CorrelatedBy<Guid>;
