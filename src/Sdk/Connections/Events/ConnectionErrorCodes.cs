namespace Sdk.Connections.Events;

/// <summary>
/// Error codes that can be associated with connection-related events, such as creation, update, or deletion failures.
/// </summary>
public static class ConnectionErrorCodes
{
    /// <summary>
    /// Error code indicating an unspecified or unknown error occurred.
    /// </summary>
    public const int UnknownError = 0;

    /// <summary>
    /// Error code indicating that adding or updating a connection failed.
    /// </summary>
    public const int AddOrUpdateConnectionFailed = 200;

    /// <summary>
    /// Error code indicating that deleting a connection failed.
    /// </summary>
    public const int DeleteConnectionFailed = 300;
}
