using Sdk.Messaging;

namespace Sdk.Connections.Commands;

public sealed record DeleteTag(Guid TagId, bool DeleteIfProtected = false) : ICommand
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
