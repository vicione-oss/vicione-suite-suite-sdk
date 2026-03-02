using MassTransit;
using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Events;

/// <summary>
/// Represents an event that is published when a user role has been successfully created.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public sealed record RoleCreatedEvent(Guid CorrelationId, Role Role) : IEvent, CorrelatedBy<Guid>;

