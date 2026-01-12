using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to delete an existing connection.
/// </summary>
public sealed record DeleteConnection(Guid ConnectionId) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
