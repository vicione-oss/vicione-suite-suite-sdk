using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// Abstract base record for response events that carry a correlation ID and optional error information.
/// </summary>
/// <remarks>
/// Concrete response events should inherit from this record and add any additional payload properties.
/// The <see cref="CorrelationId"/> is used to match the response event back to the originating command.
/// </remarks>
public abstract record ResponseEventBase : IEvent, CorrelatedBy<Guid>
{
    /// <summary>
    /// Gets or initializes the correlation ID that ties this response event to its originating command.
    /// </summary>
    public required Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets or sets error details when the operation failed, or <see langword="null"/> on success.
    /// </summary>
    public ErrorInfo? ErrorInfo { get; set; }
}
