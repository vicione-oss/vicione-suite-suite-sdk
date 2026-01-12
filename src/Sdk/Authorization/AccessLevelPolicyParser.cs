using System.Diagnostics.CodeAnalysis;

namespace Sdk.Authorization;

/// <summary>
/// Parser for access level policies
/// </summary>
public static class AccessLevelPolicyParser
{
    /// <returns>
    /// <see langword="true"/> when the given <paramref name="policy"/> could be parsed into
    /// <paramref name="accessLevelAuthorizationRequirement"/>, otherwise <see langword="false"/>
    /// </returns>
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
            accessLevelAuthorizationRequirement = new AccessLevelAuthorizationRequirement(moduleId, accessLevel)
            {
                FeatureName = featureName
            };

        return accessLevelAuthorizationRequirement is not null;
    }
}
