using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to delete a connection tag.
/// </summary>
public sealed record DeleteTag(Guid TagId, bool DeleteIfProtected = false) : ICommand
{
    /// <inheritdoc cref="ICommand.CorrelationId" />
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
