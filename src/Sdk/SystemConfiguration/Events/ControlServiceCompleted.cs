using MassTransit;
using Sdk.Messaging;
using Sdk.SystemConfiguration.Commands;
using Sdk.SystemConfiguration.Contracts;

namespace Sdk.SystemConfiguration.Events;

/// <summary>
/// Represents an event indicating that a <see cref="ControlService"/> command has completed.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public record ControlServiceCompleted(Guid CorrelationId, string ServiceName, ServiceState ServiceState) : IEvent, CorrelatedBy<Guid>;
