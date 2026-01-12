using System.Reflection;
using Sdk.Modules;

namespace Sdk.Client.Modules;

/// <summary>
/// Provides helper methods for constructing asset paths for client modules.
/// </summary>
public static class ModuleAssetHelper
{
    /// <summary>
    /// The standard prefix for referencing static web assets from a Razor Class Library.
    /// </summary>
    public const string ContentPrefix = "_content";

    /// <summary>
    /// Constructs the path for a global JavaScript file.
    /// </summary>
    /// <returns>
    /// <c>/js/<paramref name="filename"/></c> if <paramref name="relative"/> is <see langword="true"/>,
    /// otherwise <c>./js/<paramref name="filename"/></c>
    /// </returns>
    public static string GetGlobalJsPath(string filename, bool relative = false)
        => $"{Prepend(relative)}/js/{filename}";

    private static string Prepend(bool relative) => relative ? "" : ".";

    /// <summary>
    /// Constructs the path for a global CSS stylesheet file.
    /// </summary>
    /// <returns>
    /// <c>/css/<paramref name="filename"/></c> if <paramref name="relative"/> is <see langword="true"/>,
    /// otherwise <c>./css/<paramref name="filename"/></c>
    /// </returns>
    public static string GetGlobalCssPath(string filename, bool relative = false)
        => $"{Prepend(relative)}/css/{filename}";

    /// <summary>
    /// Constructs the path for a module-specific JavaScript file.
    /// </summary>
    /// <returns>
    /// <c>/{<see cref="ContentPrefix"/>}/{modulePath}/js/<paramref name="filename"/></c> if <paramref name="relative"/> is <see langword="true"/>,
    /// otherwise <c>./{<see cref="ContentPrefix"/>}/{modulePath}/js/<paramref name="filename"/></c>
    /// </returns>
    public static string GetModuleJsPath<T>(string filename, bool relative = false)
        where T : IModule
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{Prepend(relative)}/{ContentPrefix}/{modulePath}/js/{filename}";
    }

    /// <summary>
    /// Constructs the path for a module-specific CSS stylesheet file.
    /// </summary>
    /// <returns>
    /// <c>/{<see cref="ContentPrefix"/>}/{modulePath}/css/<paramref name="filename"/></c> if <paramref name="relative"/> is <see langword="true"/>,
    /// otherwise <c>./{<see cref="ContentPrefix"/>}/{modulePath}/css/<paramref name="filename"/></c>
    /// </returns>
    public static string GetModuleCssPath<T>(string filename, bool relative = false)
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{Prepend(relative)}/{ContentPrefix}/{modulePath}/css/{filename}";
    }

    /// <summary>
    /// Constructs the path for a module-specific image file.
    /// </summary>
    /// <returns>
    /// <c>/{<see cref="ContentPrefix"/>}/{modulePath}/images/<paramref name="filename"/></c> if <paramref name="relative"/> is <see langword="true"/>,
    /// otherwise <c>./{<see cref="ContentPrefix"/>}/{modulePath}/images/<paramref name="filename"/></c>
    /// </returns>
    public static string GetModuleImagePath<T>(string filename, bool relative = true)
        where T : IModule
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{Prepend(relative)}/{ContentPrefix}/{modulePath}/images/{filename}";
    }

    /// <summary>
    /// Constructs the path for a module-specific SVG icon file.
    /// </summary>
    /// <returns>
    /// <c>/{<see cref="ContentPrefix"/>}/{modulePath}/svg/<paramref name="iconName"/></c>
    /// </returns>
    public static string GetModuleIconPath<T>(string iconName)
        where T : IModule
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{ContentPrefix}/{modulePath}/svg/{iconName}";
    }

    /// <summary>
    /// Gets the manifest name for a module based on its type.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the assembly for the given type cannot be retrieved.</exception>
    public static string GetModuleManifestName(Type moduleType)
    {
        var asse = Assembly.GetAssembly(moduleType);

        return asse is null
            ? throw new ArgumentException($"Failed to retrieve assembly for type {moduleType}")
            : GetModuleManifestName(asse);
    }

    /// <summary>
    /// Gets the manifest name for a module based on its assembly.
    /// </summary>
    public static string GetModuleManifestName(Assembly moduleAssembly)
    {
        try
        {
            // todo if assembly was loaded via zip this one fails
            var calling = moduleAssembly.GetName().Name ?? moduleAssembly.ManifestModule.Name;
            if (string.IsNullOrEmpty(calling))
                throw new InvalidDataException("Calling manifest not found!");

            if (!calling.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                return calling;

            // cut of *.dll
            return calling[..^4];
        }
        catch (InvalidDataException)
        {
            // this happens on wasm assemblies that are not forced to create a manifest 
            //Console.WriteLine("Failed to get manifest from assembly {0}", moduleAssembly.FullName);
        }

        var name = moduleAssembly.GetName();

        return name.Name ?? string.Empty;
    }
}
