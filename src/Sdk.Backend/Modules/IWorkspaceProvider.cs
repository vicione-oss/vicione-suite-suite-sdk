namespace Sdk.Backend.Modules;

/// <summary>
/// Defines a provider for module-specific workspace paths.
/// </summary>
public interface IWorkspaceProvider<T> where T : BackendModule
{
    /// <summary>
    /// Gets the path to the module's home directory.
    /// </summary>
    /// <remarks>
    /// This path is intended for persistent data that cannot be restored by the initialization process.
    /// </remarks>
    string Home { get; }

    /// <summary>
    /// Gets the path to the module's cache directory.
    /// </summary>
    /// <remarks>
    /// Use this path to store temporary module data like logs or files that can be downloaded on startup or otherwise recreated.
    /// </remarks>
    string Cache { get; }
}

