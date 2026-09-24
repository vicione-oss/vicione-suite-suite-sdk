namespace Sdk.Authorization;

/// <summary>
/// Defines custom claim types used within the suite for authorization purposes.
/// </summary>
/// <remarks>
/// Claim types are URIs, as described in the
/// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claim.type?#remarks">Claim.Type remarks</see>.
/// </remarks>
[ExcludeFromCodeCoverage]
public static class SuiteClaimTypes
{
    /// <summary>
    /// The base namespace for all custom claim types defined in the suite.
    /// </summary>
    internal const string ClaimTypeNamespace = "http://schemas.vicione.com/ws/2025/03/identity/claims";

    /// <summary>
    /// A claim type used to specify module-level authorizations. The value of this claim
    /// contains a serialized representation of <see cref="ModuleAuthorizationClaimValue"/>.
    /// </summary>
    public const string ModuleAuthorization = ClaimTypeNamespace + "/moduleauthorization";
}
