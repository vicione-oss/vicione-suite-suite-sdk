namespace Sdk.UserManagement.Contracts;

/// <summary>
/// Represents a user role, which defines a set of permissions.
/// </summary>
[ExcludeFromCodeCoverage]
public class Role
{
    /// <summary>
    /// Gets or sets the unique name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of the role's purpose.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the role is managed by the system.
    /// </summary>
    /// <remarks>
    /// Managed roles cannot be deleted or modified.
    /// </remarks>
    public bool Managed { get; set; }

    /// <summary>
    /// Gets or sets the list of claims associated with this role.
    /// </summary>
    public List<UserManagementClaim> Claims { get; set; } = [];
}
