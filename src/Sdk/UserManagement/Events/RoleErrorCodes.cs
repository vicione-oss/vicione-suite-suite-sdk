namespace Sdk.UserManagement.Events;

/// <summary>
/// Represents possible error codes that can be associated with role-related events, such as creation, update, or deletion failures.
/// </summary>
public static class RoleErrorCodes
{
    /// <summary>
    /// An unknown or unspecified error occurred.
    /// </summary>
    public const int UnknownError = 0;

    /// <summary>
    /// The requested role could not be found.
    /// </summary>
    public const int NotFound = 1;

    /// <summary>
    /// The operation to create a new role failed for a generic reason.
    /// </summary>
    public const int CreateFailed = 100;

    /// <summary>
    /// The create operation failed because a role with the same name already exists.
    /// </summary>
    public const int CreateFailedAlreadyExists = 101;

    /// <summary>
    /// The operation to update a role failed for a generic reason.
    /// </summary>
    public const int UpdateFailed = 200;

    /// <summary>
    /// The update operation failed because the specified role could not be found.
    /// </summary>
    public const int UpdateFailedNotFound = 201;

    /// <summary>
    /// The operation to delete a role failed for a generic reason.
    /// </summary>
    public const int DeleteFailed = 300;

    /// <summary>
    /// The delete operation failed because the specified role could not be found.
    /// </summary>
    public const int DeleteFailedNotFound = 301;
}
