using System.Security.Claims;
using System.Text.Json;

namespace Sdk.Authorization;

/// <summary>
/// Provides factory methods for creating module authorization claims.
/// </summary>
public sealed class ModuleAuthorizationClaimFactory
{
    /// <summary>
    /// Creates a new module authorization claim for a specific feature.
    /// </summary>
    public static Claim CreateClaim(string moduleId, AccessLevel accessLevel, string featureName)
    {
        var claimValue = JsonSerializer.Serialize(
            new ModuleAuthorizationClaimValue { ModuleId = moduleId, AccessLevel = accessLevel, FeatureName = featureName },
            ClaimValueJsonSerializerContext.Default.ModuleAuthorizationClaimValue);

        var claim = new Claim(type: SuiteClaimTypes.ModuleAuthorization, claimValue);

        return claim;
    }
}
