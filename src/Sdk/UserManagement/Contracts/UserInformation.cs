namespace Sdk.UserManagement.Contracts;

/// <summary>
/// Represents basic information about a user.
/// </summary>
public record UserInformation(string UserName, string Email, string? FirstName, string? LastName);
