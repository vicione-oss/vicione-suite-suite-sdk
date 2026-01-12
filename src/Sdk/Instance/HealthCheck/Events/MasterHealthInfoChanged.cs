using Sdk.Messaging;

namespace Sdk.Instance.HealthCheck.Events;

[ForwardToUI]
public sealed record MasterHealthInfoChanged(bool IsMasterReachable) : IInstanceEvent;
