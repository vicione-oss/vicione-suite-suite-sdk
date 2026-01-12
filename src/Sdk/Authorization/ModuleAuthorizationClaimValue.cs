using System.Text.Json.Serialization;

namespace Sdk.Authorization;

/// <summary>
/// Represents the value of a module authorization claim.
/// </summary>
public sealed record ModuleAuthorizationClaimValue
{
    /// <summary>
    /// Gets the unique identifier of the module for which access is granted.
    /// </summary>
    public required string ModuleId { get; init; }

    /// <summary>
    /// Name of the feature within the module, or an empty string if access applies to the entire module
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // for compatibility
    public string FeatureName { get; init; } = string.Empty;

    /// <summary>
    /// Specifies the required access level for the module feature (if <see cref="FeatureName"/> is set) or for the module (if <see cref="FeatureName"/> is not set).
    /// </summary>
    public required AccessLevel AccessLevel { get; init; }
}
