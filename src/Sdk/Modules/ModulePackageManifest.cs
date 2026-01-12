namespace Sdk.Modules;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Object will be serialized")]
public class ModulePackageManifest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public List<ModuleDependencyPackage> Packages { get; set; } = [];
}

