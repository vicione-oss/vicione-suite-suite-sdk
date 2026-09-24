namespace Sdk.Backend.Modules;

/// <summary>
/// Lets a module clean up before the host restarts the system for a factory reset or a backup restore.
/// Register an implementation with <c>AddModuleHostRequestHandler</c>.
/// </summary>
public interface IModuleHostRequestHandler
{
    /// <summary>
    /// Called when a reset to factory settings is requested, before the system restarts, e.g. to off-board from moneo.
    /// </summary>
    Task OnReset(CancellationToken stoppingToken = default);

    /// <summary>
    /// Called when a backup restore is requested, before the system restarts, e.g. to stop engine hosts.
    /// </summary>
    Task OnRestore(CancellationToken stoppingToken = default);
}
