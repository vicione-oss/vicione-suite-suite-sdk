namespace Sdk.Backend.Modules;

public interface IModuleHostRequestHandler
{
    /// <summary>
    /// This callback is triggered when reset to factory settings is requested.
    /// Module can react on to do cleanup work before system will be restarted
    /// like perform moneo offboarding
    /// </summary>   
    Task OnReset(CancellationToken stoppingToken = default);

    /// <summary>
    /// This callback is triggered when restoring a backup was requested.
    /// Module can react on to do cleanup work before system will be restarted
    /// like stopping EngineHosts etc.
    /// </summary    
    Task OnRestore(CancellationToken stoppingToken = default);
}
