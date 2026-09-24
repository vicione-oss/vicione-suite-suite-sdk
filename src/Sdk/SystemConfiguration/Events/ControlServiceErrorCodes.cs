namespace Sdk.SystemConfiguration.Events;

/// <summary>
/// Error codes reported when controlling a system service fails.
/// </summary>
public static class ControlServiceErrorCodes
{
    /// <summary>
    /// Error code for an unspecified or unknown error.
    /// </summary>
    public const int UnknownError = -1;

    /// <summary>
    /// Error code indicating that the target service could not be found.
    /// </summary>
    public const int ServiceNotFound = 10;

    /// <summary>
    /// Error code indicating that the restart command is not supported by the service.
    /// </summary>
    public const int RestartUnsupported = 20;

    /// <summary>
    /// Error code indicating that the control service functionality is unavailable.
    /// </summary>
    public const int ControlServiceUnavailable = 30;
}
