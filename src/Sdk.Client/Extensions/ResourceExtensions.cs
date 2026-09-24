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
    extension(List<Resource> list)
    {
        /// <summary>
        /// Adds the global script <c>/js/{filename}</c>; global resources stay loaded when the component is disposed.
        /// </summary>
        public List<Resource> AddGlobalScript(string? bundle, string filename)
        {
            list.Add(ResourceFactory.CreateGlobalScript(bundle, filename));
            return list;
        }

        /// <summary>
        /// Adds the global stylesheet <c>/css/{filename}</c>; global resources stay loaded when the component is disposed.
        /// </summary>
        public List<Resource> AddGlobalStylesheet(string? bundle, string filename)
        {
            list.Add(ResourceFactory.CreateGlobalStylesheet(bundle, filename));
            return list;
        }

        /// <summary>
        /// Adds the module script <c>/_content/{assembly of T}/js/{filename}</c>; <paramref name="forceGlobal"/> keeps it loaded.
        /// </summary>
        public List<Resource> AddModuleScript<T>(string? bundle, string filename, bool forceGlobal = false)
            where T : IModule
        {
            list.Add(ResourceFactory.CreateModuleScript<T>(bundle, filename, forceGlobal));
            return list;
        }

        /// <summary>
        /// Adds the module stylesheet <c>/_content/{assembly of T}/css/{filename}</c>; <paramref name="forceGlobal"/> keeps it loaded.
        /// </summary>
        public List<Resource> AddModuleStylesheet<T>(string? bundle, string filename, bool forceGlobal = false)
            where T : IModule
        {
            list.Add(ResourceFactory.CreateModuleStylesheet<T>(bundle, filename, forceGlobal));
            return list;
        }

        /// <summary>
        /// Adds a script by the path of <paramref name="scriptUri"/>, which must be absolute.
        /// </summary>
        public List<Resource> AddScript(string? bundle, Uri scriptUri) => list.AddScript(bundle, scriptUri.AbsolutePath);

        /// <summary>
        /// Adds a script by its relative URL.
        /// </summary>
        public List<Resource> AddScript(string? bundle, string scriptUrl)
        {
            list.Add(ResourceFactory.CreateScript(bundle, scriptUrl));
            return list;
        }

        /// <summary>
        /// Adds a stylesheet by the path of <paramref name="stylesheetUri"/>, which must be absolute.
        /// </summary>
        public List<Resource> AddStylesheet(string? bundle, Uri stylesheetUri)
            => list.AddStylesheet(bundle, stylesheetUri.AbsolutePath);

        /// <summary>
        /// Adds a stylesheet by its relative URL.
        /// </summary>
        public List<Resource> AddStylesheet(string? bundle, string stylesheetUrl)
        {
            list.Add(ResourceFactory.CreateStylesheet(bundle, stylesheetUrl));
            return list;
        }

        /// <summary>
        /// Adds a script from the <c>wwwroot</c> folder of <paramref name="assembly"/>: <c>/_content/{assembly}/{relativePath}</c>.
        /// </summary>
        public List<Resource> AddComponentScript(string? bundle,
            string relativePath,
            Assembly assembly)
        {
            list.Add(ResourceFactory.CreateComponentScript(bundle, relativePath, assembly));
            return list;
        }

        /// <summary>
        /// Adds a stylesheet from the <c>wwwroot</c> folder of <paramref name="assembly"/>: <c>/_content/{assembly}/{relativePath}</c>.
        /// </summary>
        public List<Resource> AddComponentStylesheet(string? bundle,
            string relativePath,
            Assembly assembly)
        {
            list.Add(ResourceFactory.CreateComponentStylesheet(bundle, relativePath, assembly));
            return list;
        }
    }
}
