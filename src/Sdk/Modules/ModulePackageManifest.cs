namespace Sdk.Modules;

/// <summary>
/// Represents the manifest for a module package, containing metadata and a list of dependencies.
/// </summary>
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Object will be serialized")]
[ExcludeFromCodeCoverage]
public class ModulePackageManifest
{
    /// <summary>
    /// Gets or sets the name of the module package.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a brief description of the module package.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the module package was last modified.
    /// </summary>
    public DateTimeOffset? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the list of package dependencies required by this module.
    /// </summary>
    public List<ModuleDependencyPackage> Packages { get; set; } = [];
}

