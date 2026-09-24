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
    /// Loads the assembly through the <see cref="AssemblyLoadContext"/> of <typeparamref name="TModule"/> if it is listed in
    /// <paramref name="shared"/>, reusing a copy that context has already loaded; returns <see langword="null"/> otherwise.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the module's load context does not exist yet.</exception>
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

        var sharedAssembly = moduleContext.Assemblies.FirstOrDefault(a => Equals(a.GetName().Name, assemblyName.Name));
        if (sharedAssembly is not null)
        {
            return sharedAssembly;
        }

        return moduleContext.LoadFromAssemblyName(assemblyName);
    }
}
