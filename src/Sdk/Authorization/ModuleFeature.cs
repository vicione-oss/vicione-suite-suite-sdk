using Sdk.Modules;

namespace Sdk.Authorization;

/// <inheritdoc cref="IModuleFeature"/>
public record ModuleFeature<TModule>(string Name, string Description)
    : ModuleFeature(ModuleIdResolver.ResolveId<TModule>(), Name, Description)
    where TModule : IModule;

/// <inheritdoc cref="IModuleFeature"/>
public record ModuleFeature(string ModuleId, string Name, string Description) : IModuleFeature
{
    /// <inheritdoc />
    public IEnumerable<string> Path { get; init; } = [];
}

/// <summary>
/// Represents a feature within a module.
/// </summary>
public interface IModuleFeature
{
    /// <summary>
    /// Unique identifier of the module to which this feature belongs
    /// </summary>
    string ModuleId { get; }

    /// <summary>
    /// Unique name of the feature within the module
    /// </summary>
    string Name { get; }

    /// <summary>
    /// User-friendly description of the feature
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Optional path used to group features in the UI
    /// </summary>
    IEnumerable<string> Path { get; }
}
