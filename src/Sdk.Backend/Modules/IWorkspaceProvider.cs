namespace Sdk.Backend.Modules;

public interface IWorkspaceProvider<T> where T : BackendModule
{
    /// <summary>
    /// Path that points to the home directory of a module. Intended for data that cannnot be restored by the initialization process should be stored there.
    /// </summary>
    string Home { get; }

    /// <summary>
    /// Use this path to store temporary module data like logs or files that can be downloaded on startup or otherwise recreated.
    /// </summary>
    string Cache { get; }
}

