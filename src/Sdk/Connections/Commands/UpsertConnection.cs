using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to create a new connection or update an existing one.
/// </summary>
public sealed record UpsertConnection(Connection Connection) : ICommand
{
    /// <inheritdoc cref="ICommand.CorrelationId" />
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
