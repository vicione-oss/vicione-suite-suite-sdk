namespace Sdk.Authorization.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ModuleAuthorizeAttribute"/> to extract authorization requirements.
/// </summary>
public static class ModuleAuthorizeAttributeExtensions
{
    /// <summary>
    /// Extracts the <see cref="AccessLevelAuthorizationRequirement"/> from <paramref name="moduleAuthorizeAttribute"/>.
    /// </summary>
    [Obsolete("Use " + nameof(GetAccessLevelAuthorizationRequirement) + " instead. This method will be removed in a future release.")]
    public static AccessLevelAuthorizationRequirement? GetAccessLevelRequirement(this ModuleAuthorizeAttribute? moduleAuthorizeAttribute)
        => moduleAuthorizeAttribute.GetAccessLevelAuthorizationRequirement();

    /// <summary>
    /// Extracts the <see cref="AccessLevelAuthorizationRequirement"/> from <paramref name="moduleAuthorizeAttribute"/>.
    /// </summary>
    public static AccessLevelAuthorizationRequirement? GetAccessLevelAuthorizationRequirement(this ModuleAuthorizeAttribute? moduleAuthorizeAttribute)
    {
        var policy = moduleAuthorizeAttribute?.Policy;

        if (policy is not null && AccessLevelPolicyParser.TryParse(policy, out var accessLevelAuthorizationRequirement))
            return accessLevelAuthorizationRequirement;

        return null;
    }
}
