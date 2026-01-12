using System.Reflection;

namespace Sdk.Modules;

/// <summary>
/// Provides utilities for resolving a module's identifier from its assembly name
/// based on naming conventions such as <c>.Backend</c>, <c>.Client</c>, <c>.Internal</c>, or <c>.Public</c>.
/// </summary>
public static class ModuleIdResolver
{
    /// <summary>
    /// Represents the suffix for an internal module.
    /// </summary>
    public const string ModuleSuffixInternal = ".Internal";

    /// <summary>
    /// Represents the suffix for a public-facing module.
    /// </summary>
    public const string ModuleSuffixPublic = ".Public";

    /// <summary>
    /// Represents the suffix for a backend module.
    /// </summary>
    public const string ModuleSuffixBackend = ".Backend";

    /// <summary>
    /// Represents the suffix for a client module.
    /// </summary>
    public const string ModuleSuffixClient = ".Client";

    /// <summary>
    /// Resolves the module ID based on the type of a module.
    /// </summary>
    /// <typeparam name="TModule">The module type, which must implement <see cref="IModule"/>.</typeparam>
    /// <returns>The module ID derived from the module's assembly name.</returns>
    public static string ResolveId<TModule>() where TModule : IModule
        => ResolveId(typeof(TModule));

    /// <summary>
    /// Resolves the module ID from a given type's assembly name by removing the recognized suffix.
    /// </summary>
    /// <param name="moduleType">The type whose assembly name will be used to resolve the module ID.</param>
    /// <returns>The resolved module ID.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the assembly name is null or does not match expected suffix conventions.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if the module belongs to an unsupported type such as <c>.Internal</c> or <c>.Public</c>.
    /// </exception>
    public static string ResolveId(Type moduleType)
    {
        var assemblyName = moduleType.Assembly.GetName().Name
            ?? throw new InvalidOperationException($"Failed to get assembly name for type '{moduleType}'");

        if (assemblyName.EndsWith(ModuleSuffixBackend, StringComparison.Ordinal))
            return assemblyName.Replace(ModuleSuffixBackend, "", StringComparison.Ordinal);

        if (assemblyName.EndsWith(ModuleSuffixClient, StringComparison.Ordinal))
            return assemblyName.Replace(ModuleSuffixClient, "", StringComparison.Ordinal);

        if (assemblyName.EndsWith(ModuleSuffixInternal, StringComparison.Ordinal))
            throw new NotSupportedException($"Internal part '{assemblyName}' does not support Client|Backend module");

        if (assemblyName.EndsWith(ModuleSuffixPublic, StringComparison.Ordinal))
            throw new NotSupportedException($"Public part '{assemblyName}' does not support Client|Backend module");

        throw new InvalidOperationException($"Assembly '{assemblyName}' does not fit suite module naming conventions");
    }

    /// <summary>
    /// Resolves the module ID based on a given assembly.
    /// </summary>
    /// <param name="moduleAssembly">The assembly to resolve from.</param>
    /// <returns>The resolved module ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the assembly name is null.</exception>
    public static string ResolveId(Assembly moduleAssembly)
    {
        var assemblyName = moduleAssembly.GetName().Name
            ?? throw new InvalidOperationException("Failed to get assembly name");

        return ResolveId(assemblyName);
    }

    /// <summary>
    /// Resolves the module ID from a raw assembly name by removing a known suffix.
    /// </summary>
    /// <param name="moduleAssemblyName">The name of the module's assembly.</param>
    /// <returns>The base module ID without its suffix.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the assembly name does not end with a known suffix.
    /// </exception>
    public static string ResolveId(string moduleAssemblyName)
    {
        if (moduleAssemblyName.EndsWith(ModuleSuffixBackend, StringComparison.Ordinal))
            return moduleAssemblyName.Replace(ModuleSuffixBackend, "", StringComparison.Ordinal);

        if (moduleAssemblyName.EndsWith(ModuleSuffixClient, StringComparison.Ordinal))
            return moduleAssemblyName.Replace(ModuleSuffixClient, "", StringComparison.Ordinal);

        if (moduleAssemblyName.EndsWith(ModuleSuffixInternal, StringComparison.Ordinal))
            return moduleAssemblyName.Replace(ModuleSuffixInternal, "", StringComparison.Ordinal);

        if (moduleAssemblyName.EndsWith(ModuleSuffixPublic, StringComparison.Ordinal))
            return moduleAssemblyName.Replace(ModuleSuffixPublic, "", StringComparison.Ordinal);

        throw new InvalidOperationException($"Assembly '{moduleAssemblyName}' does not fit suite module naming conventions");
    }

    /// <summary>
    /// Extracts the short module name (e.g., <c>MyModule</c>) from a fully-qualified module ID (e.g., <c>Suite.Core.MyModule</c>).
    /// </summary>
    /// <param name="moduleId">The full module ID.</param>
    /// <returns>The last segment of the module ID.</returns>
    public static string GetModuleName(string moduleId) => moduleId.Split('.').Last();
}
