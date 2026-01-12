using MassTransit;
using Sdk.Messaging;

namespace Sdk.SystemConfiguration;

[ForwardToUI]
public record ControlServiceError(Guid CorrelationId, string ServiceName, ErrorInfo RequestError) : IEvent, CorrelatedBy<Guid>
{
    public const int UnknownError = -1;
    public const int ServiceNotFound = 10;
    public const int RestartUnsupported = 20;
}
