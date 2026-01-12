using System.Reflection;
using Sdk.Client.Contracts;
using Sdk.Client.Factories;
using Sdk.Modules;

namespace Sdk.Client.Extensions;

public static class ResourceExtensions
{
    /// <summary>
    /// Resource.Url -> /js/{filename}
    /// </summary>    
    public static List<Resource> AddGlobalScript(this List<Resource> list, string? bundle, string filename)
    {
        list.Add(ResourceFactory.CreateGlobalScript(bundle, filename));
        return list;
    }

    /// <summary>
    /// Resource.Url -> /css/{filename}
    /// </summary>
    public static List<Resource> AddGlobalStylesheet(this List<Resource> list, string? bundle, string filename)
    {
        list.Add(ResourceFactory.CreateGlobalStylesheet(bundle, filename));
        return list;
    }

    /// <summary>
    /// Resource.Url -> [module]/wwwroot/js/{filename}
    /// </summary>    
    public static List<Resource> AddModuleScript<T>(this List<Resource> list, string? bundle, string filename, bool forceGlobal = false)
        where T : IModule
    {
        list.Add(ResourceFactory.CreateModuleScript<T>(bundle, filename, forceGlobal));
        return list;
    }

    /// <summary>
    /// Resource.Url -> [module]/wwwroot/css/{filename}
    /// </summary>    
    public static List<Resource> AddModuleStylesheet<T>(this List<Resource> list, string? bundle, string filename, bool forceGlobal = false)
        where T : IModule
    {
        list.Add(ResourceFactory.CreateModuleStylesheet<T>(bundle, filename, forceGlobal));
        return list;
    }

    public static List<Resource> AddScript(this List<Resource> list, string? bundle, Uri scriptUri) => list.AddScript(bundle, scriptUri.AbsolutePath);

    /// <summary>
    /// adds a new resource with the given scriptUrl
    /// </summary>    
    public static List<Resource> AddScript(this List<Resource> list, string? bundle, string scriptUrl)
    {
        list.Add(ResourceFactory.CreateScript(bundle, scriptUrl));
        return list;
    }

    public static List<Resource> AddStylesheet(this List<Resource> list, string? bundle, Uri stylesheetUri)
        => list.AddStylesheet(bundle, stylesheetUri.AbsolutePath);

    /// <summary>
    /// adds a new resource with the given stylesheetUrl
    /// </summary>    
    public static List<Resource> AddStylesheet(this List<Resource> list, string? bundle, string stylesheetUrl)
    {
        list.Add(ResourceFactory.CreateStylesheet(bundle, stylesheetUrl));
        return list;
    }

    /// <summary>
    /// adds a new resource for a script in wwwroot folder of components assembly
    /// /_content/ComponentName/relativePath
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
    /// adds a new resource for a stylesheet in wwwroot folder of components assembly
    /// /_content/ComponentName/relativePath    
    public static List<Resource> AddComponentStylesheet(this List<Resource> list,
        string? bundle,
        string relativePath,
        Assembly assembly)
    {
        list.Add(ResourceFactory.CreateComponentStylesheet(bundle, relativePath, assembly));
        return list;
    }
}
