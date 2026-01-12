using Sdk.Messaging;

namespace Sdk.Instance.Events;

/// <summary>
/// Represents an event indicating that the synchronization of an instance has failed.
/// </summary>
public sealed record InstanceSynchronizationFailed(Guid InstanceId, List<string> Errors) : IEvent;
