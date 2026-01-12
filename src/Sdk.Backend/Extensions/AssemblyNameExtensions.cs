using System.Reflection;
using System.Runtime.Loader;
using Sdk.Backend.Modules;

namespace Sdk.Backend.Extensions;

public static class AssemblyNameExtensions
{
    /// <summary>
    /// Try to load the assembly using the module context. 
    /// </summary>    
    /// <exception cref="InvalidOperationException">Throws if the module context is not initialized</exception>
    public static Assembly? LoadSharedModuleAssembly<TModule>(this AssemblyName assemblyName, IEnumerable<Assembly> shared)
        where TModule : BackendModule
    {
        if (shared.All(a => !Equals(a.GetName().Name, assemblyName.Name)))
            return null;

        var moduleContextName = typeof(TModule).Assembly.GetName().Name;
        if (string.IsNullOrEmpty(moduleContextName))
            return null;

        var moduleContext = AssemblyLoadContext.All.FirstOrDefault(k => k.Name == moduleContextName) ??
            throw new InvalidOperationException($"Context {moduleContextName} is not initialized");

        // use the existing assembly from module context
        var sharedAssembly = moduleContext.Assemblies.FirstOrDefault(a => Equals(a.GetName().Name, assemblyName.Name));
        if (sharedAssembly is not null)
        {
            return sharedAssembly;
        }

        // let the module context resolve it if not present
        return moduleContext.LoadFromAssemblyName(assemblyName);
    }
}
