namespace Sdk.Authorization;

/// <remarks>
/// https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claim.type?#remarks
/// </remarks>
public static class SuiteClaimTypes
{
    internal const string ClaimTypeNamespace = "http://schemas.vicione.com/ws/2025/03/identity/claims";

    public const string ModuleAuthorization = ClaimTypeNamespace + "/moduleauthorization";
}
