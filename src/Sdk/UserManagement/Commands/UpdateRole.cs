using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Commands;

/// <summary>
/// Represents a command to update an existing user role.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record UpdateRole(Role Role) : ICommand
{
    /// <inheritdoc cref="ICommand.CorrelationId" />
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
