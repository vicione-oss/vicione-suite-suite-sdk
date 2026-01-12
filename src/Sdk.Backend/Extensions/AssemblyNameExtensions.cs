using System.Reflection;
using System.Runtime.Loader;
using Sdk.Backend.Modules;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for <see cref="AssemblyName"/>.
/// </summary>
public static class AssemblyNameExtensions
{
    /// <summary>
    /// Tries to load an assembly from the <see cref="AssemblyLoadContext"/> of a specified backend module,
    /// treating it as a shared dependency.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the module's <see cref="AssemblyLoadContext"/> has not been initialized.</exception>
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
