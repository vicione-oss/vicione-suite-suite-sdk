namespace Sdk.Modules;

/// <summary>
/// Specifies the execution environment or context of a module.
/// </summary>
public enum ModuleType
{
    /// <summary>
    /// The module is designed to run in a client-side environment, such as a web browser or desktop application.
    /// </summary>
    Client,

    /// <summary>
    /// The module is designed to run in a backend or server-side environment.
    /// </summary>
    Backend
}
