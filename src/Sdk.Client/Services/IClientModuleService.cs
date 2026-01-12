using System.Reflection;
using System.Security.Claims;
using Sdk.Client.Modules;

namespace Sdk.Client.Services;

/// <summary>
/// todo: rework interface - only leave calls that should be useable by modules here.
/// e.g. GetModuleAssemblies is only for internal usage.
/// </summary>
public interface IClientModuleService
{
    List<ClientModule> GetModules();
    IEnumerable<Assembly> GetModuleAssemblies();
    IEnumerable<string> GetAllModuleStylesheets();
    Task InitializeServices(IServiceProvider serviceProvider);
    Task OnUserAuthenticated(IServiceProvider serviceProvider, ClaimsPrincipal user);
}
