using System.Security.Claims;

namespace Sdk.Authorization;

/// <summary>
/// Reads module authorization claims.
/// </summary>
public interface IModuleAuthorizationClaimParser
{
    /// <summary>
    /// Parses a <see cref="SuiteClaimTypes.ModuleAuthorization"/> claim; returns <see langword="false"/> for any other claim
    /// or a malformed value.
    /// </summary>
    bool TryParse(Claim claim, [MaybeNullWhen(false)] out ModuleAuthorizationClaimValue claimValue);
}
