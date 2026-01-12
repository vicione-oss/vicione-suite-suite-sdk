using MassTransit;
using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Events;

[ForwardToUI]
public sealed record TagsChanged(Guid CorrelationId, CrudAction Action, List<Tag> Tags) : IEvent, CorrelatedBy<Guid>;
