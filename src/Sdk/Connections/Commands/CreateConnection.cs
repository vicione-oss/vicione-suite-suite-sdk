using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to create a new connection.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record CreateConnection(Connection Connection) : ICommand
{
    /// <inheritdoc cref="ICommand.CorrelationId" />
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
