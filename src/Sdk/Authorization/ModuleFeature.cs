using Sdk.Modules;

namespace Sdk.Authorization;

/// <inheritdoc cref="IModuleFeature"/>
[ExcludeFromCodeCoverage]
public record ModuleFeature<TModule>(string Name, string Description)
    : ModuleFeature(ModuleIdResolver.ResolveId<TModule>(), Name, Description)
    where TModule : IModule;

/// <inheritdoc cref="IModuleFeature"/>
[ExcludeFromCodeCoverage]
public record ModuleFeature(string ModuleId, string Name, string Description) : IModuleFeature
{
    /// <inheritdoc />
    public IEnumerable<string> Path { get; init; } = [];
}
