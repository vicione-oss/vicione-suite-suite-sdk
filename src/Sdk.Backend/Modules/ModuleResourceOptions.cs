namespace Sdk.Backend.Modules;

/// <summary>
/// Configures how a module's resource files are deployed to its workspace directory.
/// </summary>
public sealed class ModuleResourceOptions
{
    /// <summary>
    /// Gets or initializes the directory holding the module's resource files, relative to the module assembly.
    /// </summary>
    public required string Directory { get; init; }

    /// <summary>
    /// Gets or initializes the copy behavior of files that match no <see cref="Rules"/> entry.
    /// Defaults to <see cref="ResourceCopyBehavior.CopyIfNotExists"/>.
    /// </summary>
    public ResourceCopyBehavior DefaultBehavior { get; init; } = ResourceCopyBehavior.CopyIfNotExists;

    /// <summary>
    /// Gets or initializes rules that override <see cref="DefaultBehavior"/> for files matching a glob pattern.
    /// Rules are evaluated in order; the first matching rule wins.
    /// </summary>
    public IReadOnlyList<ResourceRule> Rules { get; init; } = [];
}
