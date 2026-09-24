using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A command consumed on one specific instance, e.g. for a state change outside the module database.
/// </summary>
public interface IInstanceDependentCommand : IInstanceDependentMessage, CorrelatedBy<Guid>
{
    /// <summary>
    /// Gets or initializes the ID that correlates the message with its outcome; the init accessor exists for deserialization.
    /// </summary>
    new Guid CorrelationId { get; init; }

    [ExcludeFromCodeCoverage]
    Guid CorrelatedBy<Guid>.CorrelationId => CorrelationId;
}
