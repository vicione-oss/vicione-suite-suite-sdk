using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Sdk.Authorization;

/// <summary>
/// Parser for module authorization claims
/// </summary>
public interface IModuleAuthorizationClaimParser
{
    /// <returns>
    /// <see langword="true"/> when the given <paramref name="claim"/> could be parsed into
    /// <paramref name="claimValue"/>, otherwise <see langword="false"/>
    /// </returns>
    bool TryParse(Claim claim, [MaybeNullWhen(false)] out ModuleAuthorizationClaimValue claimValue);
}
