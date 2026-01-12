namespace Sdk.Authorization.Extensions;

public static class ModuleAuthorizeAttributeExtensions
{
    public static AccessLevelAuthorizationRequirement? GetAccessLevelRequirement(this ModuleAuthorizeAttribute? moduleAuthorizeAttribute)
    {
        var policy = moduleAuthorizeAttribute?.Policy;

        if (policy is not null && AccessLevelPolicyParser.TryParse(policy, out var accessLevelAuthorizationRequirement))
            return accessLevelAuthorizationRequirement;

        return null;
    }
}
