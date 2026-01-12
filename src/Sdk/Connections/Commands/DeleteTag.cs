using Sdk.Messaging;

namespace Sdk.Connections.Commands;

/// <summary>
/// Represents a command to delete a connection tag.
/// </summary>
public sealed record DeleteTag(Guid TagId, bool DeleteIfProtected = false) : ICommand
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
