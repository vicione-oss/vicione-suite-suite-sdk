using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Commands;

/// <summary>
/// Represents a command to create a new user role.
/// </summary>
public sealed record CreateRole(Role Role) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}
