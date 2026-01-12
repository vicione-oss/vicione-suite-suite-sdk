using MassTransit;
using Sdk.Messaging;

namespace Sdk.Connections.Events;

/// <summary>
/// Represents an event indicating that an error occurred during a connection operation.
/// </summary>
[ForwardToUI]
public sealed record ConnectionErrorOccured(Guid CorrelationId, ErrorInfo Error, Guid? ConnectionId) : IEvent, CorrelatedBy<Guid>
{
    /// <summary>
    /// Error code indicating an unspecified or unknown error occurred.
    /// </summary>
    public const int UnknownError = 0;

    /// <summary>
    /// Error code indicating that adding or updating a connection failed.
    /// </summary>
    public const int AddOrUpdateConnectionFailed = 200;

    /// <summary>
    /// Error code indicating that deleting a connection failed.
    /// </summary>
    public const int DeleteConnectionFailed = 300;
}
