using Microsoft.AspNetCore.Authorization;

namespace Sdk.Authorization;

/// <summary>
/// Represents an authorization requirement that checks if a user has at least a minimum access level for a specific module or feature.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AccessLevelAuthorizationRequirement(string moduleId, AccessLevel minimumAccessLevel) : IAuthorizationRequirement
{

    /// <summary>
    /// Gets the unique identifier of the module.
    /// </summary>
    public string ModuleId { get; } = moduleId;

    /// <summary>
    /// Gets or initializes the name of the feature within the module; <see langword="null"/> means the whole module.
    /// </summary>
    public string? FeatureName { get; init; }

    /// <summary>
    /// Gets the minimum required access level.
    /// </summary>
    public AccessLevel MinimumAccessLevel { get; } = minimumAccessLevel;
}
