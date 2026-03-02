using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A command that will be consumed on a specific instance
/// </summary>
public interface IInstanceDependentCommand : IInstanceDependentMessage, CorrelatedBy<Guid>
{
    /// <summary>
    /// Returns the CorrelationId for the message. Setter is required for deserialization
    /// </summary>
    new Guid CorrelationId { get; init; }

    [ExcludeFromCodeCoverage]
    Guid CorrelatedBy<Guid>.CorrelationId => CorrelationId;
}
