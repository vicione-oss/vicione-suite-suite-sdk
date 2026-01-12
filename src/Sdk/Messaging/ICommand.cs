using MassTransit;

namespace Sdk.Messaging;

/// <summary>
/// Message that is sent to the central broker and is then consumed by the Master instance only
/// </summary>
public interface ICommand : IRoutableMessage, CorrelatedBy<Guid>;
