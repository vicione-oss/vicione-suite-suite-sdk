namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the state of a Linux service.
/// </summary>
public enum ServiceState
{
    /// <summary>
    /// Represents the unknown state of a Linux service.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents the enabled state of a Linux service.
    /// </summary>
    Enabled,

    /// <summary>
    /// Represents the disabled state of a Linux service.
    /// </summary>
    Disabled
}
