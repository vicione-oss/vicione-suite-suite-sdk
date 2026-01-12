namespace Sdk.Instance;

/// <summary>
/// Defines the operational mode of an instance within a cluster.
/// </summary>
public enum InstanceType
{
    /// <summary>
    /// The instance operates independently and is not part of a cluster.
    /// </summary>
    Standalone,

    /// <summary>
    /// The instance is a secondary member of a cluster, typically replicating data from a master.
    /// </summary>
    Slave,

    /// <summary>
    /// The instance is the primary member of a cluster, responsible for coordinating other instances.
    /// </summary>
    Master
}
