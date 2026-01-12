using System.Diagnostics;

namespace Sdk.Modules;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Object will be serialized")]
[DebuggerDisplay("Name = {Name,nq}, Version = {Version,nq}")]
public class ModuleDependencyPackage
{
    public required string Name { get; set; }

    public required string Version { get; set; }

    public List<ModuleDependencyPackage>? DependingOn { get; set; }
}
