using MassTransit;
using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Events;

/// <summary>
/// Represents an event that is published when a user role has been successfully deleted.
/// </summary>
[ForwardToUI]
public sealed record RoleDeletedEvent(Guid CorrelationId, Role Role) : IEvent, CorrelatedBy<Guid>;
