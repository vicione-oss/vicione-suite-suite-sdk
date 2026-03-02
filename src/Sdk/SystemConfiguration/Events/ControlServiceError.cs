using MassTransit;
using Sdk.Messaging;
using Sdk.SystemConfiguration.Commands;

namespace Sdk.SystemConfiguration.Events;

/// <summary>
/// Represents an event indicating that an error occurred during a <see cref="ControlService"/> command.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public record ControlServiceError(Guid CorrelationId, string ServiceName, ErrorInfo RequestError) : IEvent, CorrelatedBy<Guid>
{
    /// <summary>
    /// Error code for an unspecified or unknown error.
    /// </summary>
    public const int UnknownError = -1;

    /// <summary>
    /// Error code indicating that the target service could not be found.
    /// </summary>
    public const int ServiceNotFound = 10;

    /// <summary>
    /// Error code indicating that the restart command is not supported by the service.
    /// </summary>
    public const int RestartUnsupported = 20;

    /// <summary>
    /// Error code indicating that the control service functionality is unavailable.
    /// </summary>
    public const int ControlServiceUnavailable = 20;
}
