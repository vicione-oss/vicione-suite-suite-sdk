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
    public static string ResolveId<TModule>() where TModule : IModule
        => ResolveId(typeof(TModule));

    /// <summary>
    /// Resolves the module ID from the name of the type's <c>.Backend</c> or <c>.Client</c> assembly, e.g. <c>ViciOne.Suite.Oee</c>
    /// for <c>ViciOne.Suite.Oee.Backend</c>.
    /// </summary>
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
            return TrimSuffix(assemblyName, ModuleSuffixBackend);

        if (assemblyName.EndsWith(ModuleSuffixClient, StringComparison.Ordinal))
            return TrimSuffix(assemblyName, ModuleSuffixClient);

        if (assemblyName.EndsWith(ModuleSuffixInternal, StringComparison.Ordinal))
            throw new NotSupportedException($"Internal part '{assemblyName}' does not support Client|Backend module");

        if (assemblyName.EndsWith(ModuleSuffixPublic, StringComparison.Ordinal))
            throw new NotSupportedException($"Public part '{assemblyName}' does not support Client|Backend module");

        throw new InvalidOperationException($"Assembly '{assemblyName}' does not fit suite module naming conventions");
    }

    /// <summary>
    /// Resolves the module ID based on a given assembly.
    /// </summary>
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
    /// <exception cref="InvalidOperationException">
    /// Thrown if the assembly name does not end with a known suffix.
    /// </exception>
    public static string ResolveId(string moduleAssemblyName)
    {
        if (moduleAssemblyName.EndsWith(ModuleSuffixBackend, StringComparison.Ordinal))
            return TrimSuffix(moduleAssemblyName, ModuleSuffixBackend);

        if (moduleAssemblyName.EndsWith(ModuleSuffixClient, StringComparison.Ordinal))
            return TrimSuffix(moduleAssemblyName, ModuleSuffixClient);

        if (moduleAssemblyName.EndsWith(ModuleSuffixInternal, StringComparison.Ordinal))
            return TrimSuffix(moduleAssemblyName, ModuleSuffixInternal);

        if (moduleAssemblyName.EndsWith(ModuleSuffixPublic, StringComparison.Ordinal))
            return TrimSuffix(moduleAssemblyName, ModuleSuffixPublic);

        throw new InvalidOperationException($"Assembly '{moduleAssemblyName}' does not fit suite module naming conventions");
    }

    /// <summary>
    /// Extracts the short module name (e.g. <c>MyModule</c>) from a fully-qualified module ID (e.g. <c>Suite.Core.MyModule</c>).
    /// </summary>
    public static string GetModuleName(string moduleId) => moduleId.Split('.').Last();

    // Callers have checked EndsWith; only the trailing occurrence is removed, so "A.Backend.B.Backend" keeps its inner part.
    private static string TrimSuffix(string assemblyName, string suffix)
        => assemblyName[..^suffix.Length];
}
