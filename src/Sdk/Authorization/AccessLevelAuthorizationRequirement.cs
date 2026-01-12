using Microsoft.AspNetCore.Authorization;

namespace Sdk.Authorization;

/// <summary>
/// Represents an authorization requirement that checks if a user has at least a minimum access level for a specific module or feature.
/// </summary>
public sealed class AccessLevelAuthorizationRequirement(string moduleId, AccessLevel minimumAccessLevel) : IAuthorizationRequirement
{

    /// <summary>
    /// The unique identifier of the module.
    /// </summary>
    public string ModuleId { get; } = moduleId;

    /// <summary>
    /// The name of the feature within the module.
    /// </summary>
    public string? FeatureName { get; init; }

    /// <summary>
    /// The minimum required access level.
    /// </summary>
    public AccessLevel MinimumAccessLevel { get; } = minimumAccessLevel;
}
