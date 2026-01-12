using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Commands;

public sealed record UpsertTag(Tag Tag) : ICommand
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
