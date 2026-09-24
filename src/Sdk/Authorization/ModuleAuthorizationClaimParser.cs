using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sdk.Modules;

namespace Sdk.Authorization;

internal sealed partial class ModuleAuthorizationClaimParser(ILogger<ModuleAuthorizationClaimParser> logger) : IModuleAuthorizationClaimParser
{
    public bool TryParse(Claim claim, [MaybeNullWhen(false)] out ModuleAuthorizationClaimValue claimValue)
    {
        claimValue = null;

        if (claim.Type != SuiteClaimTypes.ModuleAuthorization)
            return false;

        // Anything but a JSON object is not a module authorization claim; rejecting it here avoids a thrown exception.
        {
            if (!claim.Value.StartsWith('{'))
                return false;

            if (!claim.Value.EndsWith('}'))
                return false;
        }

        try
        {
            claimValue = JsonSerializer.Deserialize(claim.Value, ClaimValueJsonSerializerContext.Default.ModuleAuthorizationClaimValue);

            if (claimValue is null)
                return false;

            if (string.IsNullOrEmpty(claimValue.FeatureName)) // A claim from before features existed targets the feature named after the module.
                claimValue = claimValue with { FeatureName = ModuleIdResolver.GetModuleName(claimValue.ModuleId) };

            return true;
        }
        catch (Exception ex)
        {
            DeserializeClaimFailed(logger, ex, nameof(ModuleAuthorizationClaimValue));
        }

        return false;
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Claim could not be deserialized into '{JsonObjectClassName}'")]
    private static partial void DeserializeClaimFailed(ILogger<ModuleAuthorizationClaimParser> logger, Exception exception, string JsonObjectClassName);
}
