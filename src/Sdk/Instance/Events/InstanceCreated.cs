using Sdk.Messaging;

namespace Sdk.Instance.Events;

/// <summary>
/// Represents an event indicating that a new instance has been created or detected.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public sealed record InstanceCreated(Guid InstanceId) : IEvent;
