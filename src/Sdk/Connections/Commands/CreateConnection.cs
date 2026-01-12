using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to create a new connection.
/// </summary>
public sealed record CreateConnection(Connection Connection) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
