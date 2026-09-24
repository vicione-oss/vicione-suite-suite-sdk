using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A state change sent to the central broker and consumed by the master instance only; results come back as events.
/// </summary>
public interface ICommand : IRoutableMessage, CorrelatedBy<Guid>
{
    /// <summary>
    /// Gets or initializes the ID that correlates the message with its outcome; the init accessor exists for deserialization.
    /// </summary>
    new Guid CorrelationId { get; init; }

    [ExcludeFromCodeCoverage]
    Guid CorrelatedBy<Guid>.CorrelationId => CorrelationId;
}
