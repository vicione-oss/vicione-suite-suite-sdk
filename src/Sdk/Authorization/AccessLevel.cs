namespace Sdk.Authorization;

/// <summary>
/// Supported access levels for a module feature.
/// </summary>
/// <remarks>
/// It is up to the module to define what each access level means based on the general description
/// of the access level and in context of a module feature.
/// </remarks>
public enum AccessLevel
{
    /// <summary>
    /// Compared to <see cref="Full"/>, only some functionality of the module feature is available to the user.
    /// </summary>
    Partial,

    /// <summary>
    /// Full functionality of the module feature is available to the user.
    /// </summary>
    Full
}
