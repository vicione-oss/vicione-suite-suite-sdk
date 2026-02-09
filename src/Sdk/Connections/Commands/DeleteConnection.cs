using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to delete an existing connection.
/// </summary>
public sealed record DeleteConnection(Guid ConnectionId) : ICommand
{
    /// <inheritdoc cref="ICommand.CorrelationId" />
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
