using System.Reflection;
using Sdk.Client.Contracts;
using Sdk.Client.Factories;
using Sdk.Modules;

namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for a <see cref="List{T}"/> of <see cref="Resource"/> objects.
/// </summary>
public static class ResourceExtensions
{
    /// <summary>
    /// Adds a global script resource to the list. The URL will be formatted as '/js/{filename}'.
    /// </summary>
    public static List<Resource> AddGlobalScript(this List<Resource> list, string? bundle, string filename)
    {
        list.Add(ResourceFactory.CreateGlobalScript(bundle, filename));
        return list;
    }

    /// <summary>
    /// Adds a global stylesheet resource to the list. The URL will be formatted as '/css/{filename}'.
    /// </summary>
    public static List<Resource> AddGlobalStylesheet(this List<Resource> list, string? bundle, string filename)
    {
        list.Add(ResourceFactory.CreateGlobalStylesheet(bundle, filename));
        return list;
    }

    /// <summary>
    /// Adds a module-specific script resource to the list. The URL will be formatted as '/[module]/wwwroot/js/{filename}'.
    /// </summary>
    public static List<Resource> AddModuleScript<T>(this List<Resource> list, string? bundle, string filename, bool forceGlobal = false)
        where T : IModule
    {
        list.Add(ResourceFactory.CreateModuleScript<T>(bundle, filename, forceGlobal));
        return list;
    }

    /// <summary>
    /// Adds a module-specific stylesheet resource to the list. The URL will be formatted as '/[module]/wwwroot/css/{filename}'.
    /// </summary>
    public static List<Resource> AddModuleStylesheet<T>(this List<Resource> list, string? bundle, string filename, bool forceGlobal = false)
        where T : IModule
    {
        list.Add(ResourceFactory.CreateModuleStylesheet<T>(bundle, filename, forceGlobal));
        return list;
    }

    /// <summary>
    /// Adds a new script resource to the list using a URI.
    /// </summary>
    public static List<Resource> AddScript(this List<Resource> list, string? bundle, Uri scriptUri) => list.AddScript(bundle, scriptUri.AbsolutePath);

    /// <summary>
    /// Adds a new script resource to the list using a URL string.
    /// </summary>
    public static List<Resource> AddScript(this List<Resource> list, string? bundle, string scriptUrl)
    {
        list.Add(ResourceFactory.CreateScript(bundle, scriptUrl));
        return list;
    }

    /// <summary>
    /// Adds a new stylesheet resource to the list using a URI.
    /// </summary>
    public static List<Resource> AddStylesheet(this List<Resource> list, string? bundle, Uri stylesheetUri)
        => list.AddStylesheet(bundle, stylesheetUri.AbsolutePath);

    /// <summary>
    /// Adds a new stylesheet resource to the list using a URL string.
    /// </summary>
    public static List<Resource> AddStylesheet(this List<Resource> list, string? bundle, string stylesheetUrl)
    {
        list.Add(ResourceFactory.CreateStylesheet(bundle, stylesheetUrl));
        return list;
    }

    /// <summary>
    /// Adds a script resource from a component's 'wwwroot' folder. The URL will be formatted as '/_content/[ComponentName]/[relativePath]'.
    /// </summary>
    public static List<Resource> AddComponentScript(this List<Resource> list,
        string? bundle,
        string relativePath,
        Assembly assembly)
    {
        list.Add(ResourceFactory.CreateComponentScript(bundle, relativePath, assembly));
        return list;
    }

    /// <summary>
    /// Adds a stylesheet resource from a component's 'wwwroot' folder. The URL will be formatted as '/_content/[ComponentName]/[relativePath]'.
    /// </summary>
    public static List<Resource> AddComponentStylesheet(this List<Resource> list,
        string? bundle,
        string relativePath,
        Assembly assembly)
    {
        list.Add(ResourceFactory.CreateComponentStylesheet(bundle, relativePath, assembly));
        return list;
    }
}
