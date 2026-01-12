using Microsoft.AspNetCore.Authorization;

namespace Sdk.Authorization;

public sealed class AccessLevelAuthorizationRequirement(string moduleId, AccessLevel minimumAccessLevel) : IAuthorizationRequirement
{
    public string ModuleId { get; } = moduleId;
    public string? FeatureName { get; init; }
    public AccessLevel MinimumAccessLevel { get; } = minimumAccessLevel;
}
