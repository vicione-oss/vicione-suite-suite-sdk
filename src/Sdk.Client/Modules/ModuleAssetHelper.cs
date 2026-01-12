using System.Reflection;
using Sdk.Modules;

namespace Sdk.Client.Modules;

public static class ModuleAssetHelper
{
    public const string ContentPrefix = "_content";

    /// <summary>
    /// $"./js/{filename}"
    /// </summary>
    public static string GetGlobalJsPath(string filename, bool relative = false)
        => $"{Prepend(relative)}/js/{filename}";

    private static string Prepend(bool relative) => relative ? "" : ".";

    /// <summary>
    /// $"./js/{filename}"
    /// </summary>
    public static string GetGlobalCssPath(string filename, bool relative = false)
        => $"{Prepend(relative)}/css/{filename}";

    /// <summary>
    /// $"_content/{modulePath}/js/{filename}"
    /// </summary>
    public static string GetModuleJsPath<T>(string filename, bool relative = false)
        where T : IModule
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{Prepend(relative)}/{ContentPrefix}/{modulePath}/js/{filename}";
    }

    /// <summary>
    /// $"_content/{modulePath}/css/{filename}"
    /// </summary>
    public static string GetModuleCssPath<T>(string filename, bool relative = false)
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{Prepend(relative)}/{ContentPrefix}/{modulePath}/css/{filename}";
    }

    /// <summary>
    /// $"_content/{modulePath}/images/{filename}"
    /// </summary>
    public static string GetModuleImagePath<T>(string filename, bool relative = true)
        where T : IModule
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{Prepend(relative)}/{ContentPrefix}/{modulePath}/images/{filename}";
    }

    /// <summary>
    /// $"_content/{modulePath}/svg/{iconName}"
    /// </summary>
    public static string GetModuleIconPath<T>(string iconName)
        where T : IModule
    {
        var modulePath = GetModuleManifestName(typeof(T));

        return $"{ContentPrefix}/{modulePath}/svg/{iconName}";
    }

    public static string GetModuleManifestName(Type moduleType)
    {
        var asse = Assembly.GetAssembly(moduleType);

        return asse is null
            ? throw new ArgumentException($"Failed to retrieve assembly for type {moduleType}")
            : GetModuleManifestName(asse);
    }

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
