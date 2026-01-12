using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// A command that will be consumed on a specific instance
/// </summary>
public interface IInstanceDependentCommand : IInstanceDependentMessage, CorrelatedBy<Guid>;
