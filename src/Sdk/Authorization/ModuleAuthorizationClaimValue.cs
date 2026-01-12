using System.Text.Json.Serialization;

namespace Sdk.Authorization;

public sealed record ModuleAuthorizationClaimValue
{
    public required string ModuleId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // for compatibility
    public string FeatureName { get; init; } = string.Empty;
    public required AccessLevel AccessLevel { get; init; }
}
