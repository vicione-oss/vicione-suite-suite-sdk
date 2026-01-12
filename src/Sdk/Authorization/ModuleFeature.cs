using Sdk.Modules;

namespace Sdk.Authorization;

public record ModuleFeature<TModule>(string Name, string Description)
    : ModuleFeature(ModuleIdResolver.ResolveId<TModule>(), Name, Description)
    where TModule : IModule;

/// <summary>
/// </summary>
/// <param name="ModuleId"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
public record ModuleFeature(string ModuleId, string Name, string Description) : IModuleFeature
{
    /// <inheritdoc />
    public IEnumerable<string> Path { get; init; } = [];
}

public interface IModuleFeature
{
    /// <summary>
    ///     Context of the feature
    /// </summary>
    string ModuleId { get; }

    /// <summary>
    ///     Unique feature name
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Describes the feature to the user
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Optionally groups features in the UI
    /// </summary>
    IEnumerable<string> Path { get; }
}
