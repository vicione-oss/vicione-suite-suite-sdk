using Sdk.Messaging;

namespace Sdk.Instance.HealthCheck.Events;

/// <summary>
/// Represents an event indicating a change in the reachability of the master instance.
/// </summary>
[ExcludeFromCodeCoverage]
[ForwardToUI]
public sealed record MasterHealthInfoChanged(bool IsMasterReachable) : IInstanceEvent;
