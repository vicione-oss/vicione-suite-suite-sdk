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
    Task<IReadOnlyCollection<ModuleMetadata>> GetInstalledModules(CancellationToken token = default);

    /// <summary>
    /// Retrieves information about all instances currently registered in the same cluster.
    /// </summary>
    Task<List<IInstanceInformation>> GetInstancesInCluster(CancellationToken token = default);
}
