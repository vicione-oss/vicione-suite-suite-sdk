using System.Security.Claims;
using System.Text.Json;

namespace Sdk.Authorization;

public sealed class ModuleAuthorizationClaimFactory
{
    public static Claim CreateClaim(string moduleId, AccessLevel accessLevel, string featureName)
    {
        var claimValue = JsonSerializer.Serialize(
            new ModuleAuthorizationClaimValue { ModuleId = moduleId, AccessLevel = accessLevel, FeatureName = featureName },
            ClaimValueJsonSerializerContext.Default.ModuleAuthorizationClaimValue);

        var claim = new Claim(type: SuiteClaimTypes.ModuleAuthorization, claimValue);

        return claim;
    }
}
