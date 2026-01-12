using Sdk.Modules;

namespace Sdk.Instance;

/// <summary>
/// Provides access to information about the current instance as well as other instances
/// in the cluster, along with metadata about installed modules.
/// </summary>
public interface IInstanceInformationProvider
{
    /// <summary>
    /// Information about the local instance (the instance where this provider is running)
    /// </summary>
    IInstanceInformation Local { get; }

    /// <summary>
    /// Retrieves metadata about all modules installed on the local instance.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.  
    /// The task result contains a read-only collection of <see cref="ModuleMetadata"/> entries
    /// describing each installed module.
    /// </returns>
    Task<IReadOnlyCollection<ModuleMetadata>> GetInstalledModules();

    /// <summary>
    /// Retrieves information about all instances currently registered in the same cluster.
    /// </summary>
    /// <param name="token">
    /// A cancellation token that can be used to cancel the operation before completion.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.  
    /// The task result contains a list of <see cref="IInstanceInformation"/> objects,
    /// each describing a cluster member.
    /// </returns>
    Task<List<IInstanceInformation>> GetInstancesInCluster(CancellationToken token = default);
}
