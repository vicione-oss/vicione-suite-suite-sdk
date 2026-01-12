using MassTransit;
using Sdk.Messaging;

namespace Sdk.Connections.Events;

[ForwardToUI]
public sealed record ConnectionErrorOccured(Guid CorrelationId, ErrorInfo Error, Guid? ConnectionId) : IEvent, CorrelatedBy<Guid>
{
    public const int UnknownError = 0;
    public const int AddOrUpdateConnectionFailed = 200;
    public const int DeleteConnectionFailed = 300;
}
