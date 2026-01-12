using System.Reflection;
using System.Security.Claims;
using Sdk.Client.Modules;

namespace Sdk.Client.Services;

/// <summary>
/// Defines a service for managing client-side modules.
/// </summary>
[Obsolete("This interface will get removed soon")]
public interface IClientModuleService
{
    /// <summary>
    /// Gets a list of all registered client modules.
    /// </summary>
    List<ClientModule> GetModules();

    /// <summary>
    /// Gets the assemblies of all registered client modules.
    /// </summary>
    IEnumerable<Assembly> GetModuleAssemblies();

    /// <summary>
    /// Gets the paths to all stylesheets registered by the client modules.
    /// </summary>
    IEnumerable<string> GetAllModuleStylesheets();

    /// <summary>
    /// Initializes the services for all registered modules.
    /// </summary>
    Task InitializeServices(IServiceProvider serviceProvider);

    /// <summary>
    /// Notifies all registered modules that a user has been authenticated.
    /// </summary>
    Task OnUserAuthenticated(IServiceProvider serviceProvider, ClaimsPrincipal user);
}
