using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sdk.Instance;

/// <summary>
/// Provides information and notifications about instances in a cluster,
/// including health status updates and instance lifecycle events.
/// </summary>
public interface IClusterInformationProvider
{
    /// <summary>
    /// Occurs when a new instance is detected and added to the cluster.
    /// </summary>
    event Func<IInstanceInformation, Task>? NewInstanceAdded;

    /// <summary>
    /// Occurs when the health status of an instance in the cluster changes.
    /// </summary>
    event Func<Guid, HealthStatus, DateTimeOffset, Task>? HealthStatusChanged;

    /// <summary>
    /// Occurs when an instance is removed from the cluster.
    /// </summary>
    event Func<Guid, Task>? InstanceDeleted;

    /// <summary>
    /// Occurs when an attempt to delete an instance from the cluster fails.
    /// </summary>
    event Func<Guid, Task>? DeleteInstanceFailed;

    /// <summary>
    /// Retrieves a list of all instances currently registered in the cluster.
    /// </summary>
    Task<List<IInstanceInformation>> GetInstancesInCluster(CancellationToken token);

    /// <summary>
    /// Retrieves the current health status of a specific instance in the cluster.
    /// </summary>
    Task<HealthStatus?> GetHealthStatus(Guid instanceId, CancellationToken token = default);

    /// <summary>
    /// Checks whether the cluster as a whole is considered healthy.
    /// </summary>
    Task<bool> IsClusterHealthy(CancellationToken token = default);
}
