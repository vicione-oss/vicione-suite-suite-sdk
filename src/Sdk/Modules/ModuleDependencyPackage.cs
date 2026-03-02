using System.Diagnostics;

namespace Sdk.Modules;

/// <summary>
/// Represents a package dependency for a module, including its name, version, and transitive dependencies.
/// </summary>
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Object will be serialized")]
[DebuggerDisplay("Name = {Name,nq}, Version = {Version,nq}")]
[ExcludeFromCodeCoverage]
public class ModuleDependencyPackage
{
    /// <summary>
    /// Gets or sets the name of the dependency package.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the version of the dependency package.
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Gets or sets a list of packages that this package, in turn, depends on.
    /// </summary>
    public List<ModuleDependencyPackage>? DependingOn { get; set; }
}
