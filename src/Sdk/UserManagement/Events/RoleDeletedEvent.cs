using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Events;

/// <summary>
/// Represents an event that is published when a user role has been successfully deleted.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public sealed record RoleDeletedEvent(Role Role) : ResponseEventBase;
