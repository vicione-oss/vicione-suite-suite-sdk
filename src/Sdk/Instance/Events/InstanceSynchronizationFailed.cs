using Sdk.Messaging;

namespace Sdk.Instance.Events;

public sealed record InstanceSynchronizationFailed(Guid InstanceId, List<string> Errors) : IEvent;
