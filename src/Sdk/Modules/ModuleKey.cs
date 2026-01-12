namespace Sdk.Modules;

/// <summary>
/// Represents a unique key for identifying a module.
/// </summary>
public readonly record struct ModuleKey
{
    /// <summary>
    /// Gets the unique identifier of the module.
    /// </summary>
    public string ModuleId { get; init; }

    /// <summary>
    /// Gets the type of the module.
    /// </summary>
    public ModuleType ModuleType { get; init; }

    /// <summary>
    /// Returns a string representation of the module key.
    /// </summary>
    public override string ToString() => $"{nameof(ModuleKey)} Id:{ModuleId} Type:{ModuleType}";
}
