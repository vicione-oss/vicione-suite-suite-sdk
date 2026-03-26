namespace Sdk.Connections.Events;

/// <summary>
/// Error codes that can be associated with tag-related events, such as creation, update, or deletion failures.
/// </summary>
public static class TagErrorCodes
{
    /// <summary>
    /// Error code indicating an unspecified or unknown error occurred.
    /// </summary>
    public const int UnknownError = 0;

    /// <summary>
    /// Error code indicating that adding or updating a tag failed.
    /// </summary>
    public const int AddOrUpdateTagFailed = 200;

    /// <summary>
    /// Error code indicating that deleting a tag failed.
    /// </summary>
    public const int DeleteTagFailed = 300;
}
