using System.Text.Json.Serialization;

namespace Sdk.Authorization;

/// <summary>
/// Represents the value of a module authorization claim.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record ModuleAuthorizationClaimValue
{
    /// <summary>
    /// Gets or initializes the unique identifier of the module for which access is granted.
    /// </summary>
    public required string ModuleId { get; init; }

    /// <summary>
    /// Gets or initializes the name of the feature within the module; empty means the whole module.
    /// </summary>
    /// <remarks>
    /// <see cref="IModuleAuthorizationClaimParser"/> replaces an empty name with the module's short name.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // Claims issued before features existed omit the name.
    public string FeatureName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the access level granted for <see cref="FeatureName"/>, or for the whole module if it is empty.
    /// </summary>
    public required AccessLevel AccessLevel { get; init; }
}
