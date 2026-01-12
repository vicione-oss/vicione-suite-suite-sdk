using Sdk.Messaging;

namespace Sdk.Connections.Commands;

public sealed record DeleteConnection(Guid ConnectionId) : ICommand
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
