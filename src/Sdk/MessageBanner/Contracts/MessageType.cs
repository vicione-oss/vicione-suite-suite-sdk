namespace Sdk.MessageBanner.Contracts;

/// <summary>
/// Defines the severity level of a message banner.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// An informational message.
    /// </summary>
    Information,

    /// <summary>
    /// A warning message that indicates a potential issue.
    /// </summary>
    Warning,

    /// <summary>
    /// An error message that indicates a problem has occurred.
    /// </summary>
    Error
}
