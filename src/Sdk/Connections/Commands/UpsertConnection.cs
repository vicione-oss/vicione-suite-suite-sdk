using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to create a new connection or update an existing one.
/// </summary>
public sealed record UpsertConnection(Connection Connection) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
