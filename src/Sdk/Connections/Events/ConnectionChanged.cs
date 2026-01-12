using MassTransit;
using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Events;

[ForwardToUI]
public sealed record ConnectionChanged(Guid CorrelationId, CrudAction Action, Connection Connection, List<Tag> AddedTags, List<Tag> RemovedTags) : IEvent, CorrelatedBy<Guid>;
