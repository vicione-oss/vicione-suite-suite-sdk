using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Sdk.Authorization;

/// <summary>
/// Parser for module authorization claims
/// </summary>
public interface IModuleAuthorizationClaimParser
{
    /// <summary>
    /// Attempts to parse the specified <paramref name="claim"/> into a <see cref="ModuleAuthorizationClaimValue"/>.
    /// </summary>
    bool TryParse(Claim claim, [MaybeNullWhen(false)] out ModuleAuthorizationClaimValue claimValue);
}
