using System.Security.Claims;

namespace Sdk.UserManagement.Contracts;

/// <summary>
/// Abstraction of a claim in context of user management.
/// </summary>
/// <remarks>
/// This type was introduced because <see cref="Claim"/> is not serializable (required by MassTransit).
/// </remarks>
public readonly record struct UserManagementClaim
{
    /// <inheritdoc cref="Claim.Type"/>
    public required string Type { get; init; }

    /// <inheritdoc cref="Claim.Value"/>
    public required string Value { get; init; }
}
