using Sdk.Messaging;

namespace Sdk.Instance.Events;

[ForwardToUI]
public sealed record InstanceCreated(Guid InstanceId) : IEvent;
