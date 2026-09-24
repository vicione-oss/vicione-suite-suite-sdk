namespace Sdk.Authorization;

/// <summary>
/// Represents a feature within a module.
/// </summary>
public interface IModuleFeature
{
    /// <summary>
    /// Gets the ID of the module the feature belongs to.
    /// </summary>
    string ModuleId { get; }

    /// <summary>
    /// Gets the feature's name, unique within the module; access checks refer to it.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the user-facing description of the feature.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the path that groups the feature in the UI; empty for none.
    /// </summary>
    IEnumerable<string> Path { get; }
}
