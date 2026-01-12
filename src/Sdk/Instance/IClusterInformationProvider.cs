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
    /// <remarks>
    /// The event handler receives the <see cref="IInstanceInformation"/> for the newly added instance.
    /// </remarks>
    event Func<IInstanceInformation, Task>? NewInstanceAdded;

    /// <summary>
    /// Occurs when the health status of an instance in the cluster changes.
    /// </summary>
    /// <remarks>
    /// The event handler provides:
    /// <list type="bullet">
    /// <item><description><paramref name="Guid"/>: The ID of the instance whose health status changed.</description></item>
    /// <item><description><see cref="HealthStatus"/>: The new health status.</description></item>
    /// <item><description><see cref="DateTime"/>: The timestamp when the change was detected.</description></item>
    /// </list>
    /// </remarks>
    event Func<Guid, HealthStatus, DateTime, Task>? HealthStatusChanged;

    /// <summary>
    /// Occurs when an instance is removed from the cluster.
    /// </summary>
    /// <remarks>
    /// The event handler provides the ID of the removed instance.
    /// </remarks>
    event Func<Guid, Task>? InstanceDeleted;

    /// <summary>
    /// Occurs when an attempt to delete an instance from the cluster fails.
    /// </summary>
    /// <remarks>
    /// The event handler provides the ID of the instance that failed to delete.
    /// </remarks>
    event Func<Guid, Task>? DeleteInstanceFailed;

    /// <summary>
    /// Retrieves a list of all instances currently registered in the cluster.
    /// </summary>
    /// <param name="token">
    /// A cancellation token that can be used to cancel the operation before completion.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.  
    /// The task result contains a list of <see cref="IInstanceInformation"/> objects,
    /// each representing an instance in the cluster.
    /// </returns>
    Task<List<IInstanceInformation>> GetInstancesInCluster(CancellationToken token);

    /// <summary>
    /// Retrieves the current health status of a specific instance in the cluster.
    /// </summary>
    /// <param name="instanceId">The unique identifier of the instance to check.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.  
    /// The task result contains the <see cref="HealthStatus"/> of the instance, or <see langword="null"/> if unknown.
    /// </returns>
    Task<HealthStatus?> GetHealthStatus(Guid instanceId);

    /// <summary>
    /// Checks whether the cluster as a whole is considered healthy.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.  
    /// The task result is <see langword="true"/> if the cluster is healthy; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> IsClusterHealthy();
}
