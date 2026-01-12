using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

public sealed record CreateConnection(Connection Connection) : ICommand
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
