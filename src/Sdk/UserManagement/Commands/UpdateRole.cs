using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Commands;

/// <summary>
/// Represents a command to update an existing user role.
/// </summary>
public sealed record UpdateRole(Role Role) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}
