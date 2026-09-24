namespace Sdk.Authorization;

/// <summary>
/// Parses the policy names built by <see cref="ModulePolicyProvider"/>.
/// </summary>
public static class AccessLevelPolicyParser
{
    /// <summary>
    /// Parses a policy of the form <c>{prefix}_{moduleId}_{accessLevel}[_{featureName}]</c>; returns <see langword="false"/> for
    /// any other name. The name is split on <c>_</c>, so only the first part of a feature name containing <c>_</c> survives.
    /// </summary>
    public static bool TryParse(string policy, [MaybeNullWhen(false)] out AccessLevelAuthorizationRequirement accessLevelAuthorizationRequirement)
    {
        accessLevelAuthorizationRequirement = null;
        if (!policy.StartsWith(Constants.AuthorizationPolicyPrefix, StringComparison.Ordinal))
            return false;

        var policyTokens = policy.Split('_');
        if (policyTokens.Length < 3)
            return false;

        var moduleId = policyTokens[1];
        var accessLevelString = policyTokens[2];
        string? featureName = null;
        if (policyTokens.Length > 3)
            featureName = policyTokens[3];

        if (Enum.TryParse<AccessLevel>(accessLevelString, out var accessLevel))
        {
            accessLevelAuthorizationRequirement = new AccessLevelAuthorizationRequirement(moduleId, accessLevel)
            {
                FeatureName = featureName
            };
        }

        return accessLevelAuthorizationRequirement is not null;
    }
}
