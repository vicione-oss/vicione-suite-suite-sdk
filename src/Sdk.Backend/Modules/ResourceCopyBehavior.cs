namespace Sdk.Backend.Modules;

/// <summary>
/// Specifies how a module resource file should be handled when deploying to the module's workspace.
/// </summary>
public enum ResourceCopyBehavior
{
    /// <summary>
    /// Copy the resource only if it does not already exist in the target directory.
    /// This is the default behavior.
    /// </summary>
    CopyIfNotExists,

    /// <summary>
    /// Always copy the resource, overwriting any existing file in the target directory.
    /// </summary>
    CopyAlways,

    /// <summary>
    /// Never copy the resource. Use this for files that exist in the resource directory
    /// for development purposes but should not be deployed to the module's workspace.
    /// </summary>
    NeverCopy,
}
