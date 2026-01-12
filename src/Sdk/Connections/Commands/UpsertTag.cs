using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to create a new tag or update an existing one.
/// </summary>
public sealed record UpsertTag(Tag Tag) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
