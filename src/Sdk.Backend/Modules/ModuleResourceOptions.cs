namespace Sdk.Backend.Modules;

/// <summary>
/// Configures how a module's resource files are deployed to its workspace directory.
/// </summary>
public sealed class ModuleResourceOptions
{
    /// <summary>
    /// The name of the directory containing the module's resource files, relative to the module assembly location.
    /// </summary>
    public required string Directory { get; init; }

    /// <summary>
    /// The default copy behavior applied to all resource files that do not match any <see cref="Rules"/> entry.
    /// Defaults to <see cref="ResourceCopyBehavior.CopyIfNotExists"/>.
    /// </summary>
    public ResourceCopyBehavior DefaultBehavior { get; init; } = ResourceCopyBehavior.CopyIfNotExists;

    /// <summary>
    /// Optional rules that override <see cref="DefaultBehavior"/> for files matching a glob pattern.
    /// Rules are evaluated in order; the first matching rule wins.
    /// </summary>
    public IReadOnlyList<ResourceRule> Rules { get; init; } = [];
}
