using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Events;

/// <summary>
/// Represents an event that is published when a user role has been successfully updated.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public sealed record RoleUpdatedEvent(Role Role) : ResponseEventBase;
