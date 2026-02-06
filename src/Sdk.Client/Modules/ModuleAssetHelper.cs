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
    /// Constructs a relative URI for a global JavaScript file.
    /// </summary>
    /// <param name="filename">The name of the JavaScript file. Cannot be null or empty.</param>
    /// <param name="relative">If <see langword="true"/>, returns a URI starting with <c>/</c>;
    /// otherwise returns a URI starting with <c>./</c>.</param>
    /// <returns>
    /// A <see cref="Uri"/> with <see cref="UriKind.Relative"/> kind representing the path to the JavaScript file.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filename"/> is null or empty.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed path is not a valid relative URI.</exception>
    public static Uri GetGlobalJsUrl(string filename, bool relative = false)
    {
        ValidateFilename(filename);
        var path = $"{GetPrefix(relative)}/js/{filename}";
        return new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Constructs a relative URI for a global CSS stylesheet file.
    /// </summary>
    /// <param name="filename">The name of the CSS file. Cannot be null or empty.</param>
    /// <param name="relative">If <see langword="true"/>, returns a URI starting with <c>/</c>;
    /// otherwise returns a URI starting with <c>./</c>.</param>
    /// <returns>
    /// A <see cref="Uri"/> with <see cref="UriKind.Relative"/> kind representing the path to the CSS file.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filename"/> is null or empty.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed path is not a valid relative URI.</exception>
    public static Uri GetGlobalCssUrl(string filename, bool relative = false)
    {
        ValidateFilename(filename);
        var path = $"{GetPrefix(relative)}/css/{filename}";
        return new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Constructs a relative URI for a module-specific JavaScript file.
    /// </summary>
    /// <typeparam name="T">The module type implementing <see cref="IModule"/>.</typeparam>
    /// <param name="filename">The name of the JavaScript file. Cannot be null or empty.</param>
    /// <param name="relative">If <see langword="true"/>, returns a URI starting with <c>/</c>;
    /// otherwise returns a URI starting with <c>./</c>.</param>
    /// <returns>
    /// A <see cref="Uri"/> with <see cref="UriKind.Relative"/> kind representing the path to the module JavaScript file.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filename"/> is null or empty,
    /// or if the assembly for type <typeparamref name="T"/> cannot be retrieved.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed path is not a valid relative URI.</exception>
    public static Uri GetModuleJsUrl<T>(string filename, bool relative = false)
        where T : IModule
    {
        ValidateFilename(filename);
        var modulePath = GetModuleManifestName(typeof(T));
        var path = $"{GetPrefix(relative)}/{ContentPrefix}/{modulePath}/js/{filename}";
        return new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Constructs a relative URI for a module-specific CSS stylesheet file.
    /// </summary>
    /// <typeparam name="T">The module type implementing <see cref="IModule"/>.</typeparam>
    /// <param name="filename">The name of the CSS file. Cannot be null or empty.</param>
    /// <param name="relative">If <see langword="true"/>, returns a URI starting with <c>/</c>;
    /// otherwise returns a URI starting with <c>./</c>.</param>
    /// <returns>
    /// A <see cref="Uri"/> with <see cref="UriKind.Relative"/> kind representing the path to the module CSS file.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filename"/> is null or empty,
    /// or if the assembly for type <typeparamref name="T"/> cannot be retrieved.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed path is not a valid relative URI.</exception>
    public static Uri GetModuleCssUrl<T>(string filename, bool relative = false)
        where T : IModule
    {
        ValidateFilename(filename);
        var modulePath = GetModuleManifestName(typeof(T));
        var path = $"{GetPrefix(relative)}/{ContentPrefix}/{modulePath}/css/{filename}";
        return new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Constructs a relative URI for a module-specific image file.
    /// </summary>
    /// <typeparam name="T">The module type implementing <see cref="IModule"/>.</typeparam>
    /// <param name="filename">The name of the image file. Cannot be null or empty.</param>
    /// <param name="relative">If <see langword="true"/>, returns a URI starting with <c>/</c>;
    /// otherwise returns a URI starting with <c>./</c>. Defaults to <see langword="true"/>.</param>
    /// <returns>
    /// A <see cref="Uri"/> with <see cref="UriKind.Relative"/> kind representing the path to the module image file.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filename"/> is null or empty,
    /// or if the assembly for type <typeparamref name="T"/> cannot be retrieved.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed path is not a valid relative URI.</exception>
    public static Uri GetModuleImageUrl<T>(string filename, bool relative = true)
        where T : IModule
    {
        ValidateFilename(filename);
        var modulePath = GetModuleManifestName(typeof(T));
        var path = $"{GetPrefix(relative)}/{ContentPrefix}/{modulePath}/images/{filename}";
        return new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Constructs a relative URI for a module-specific SVG icon file.
    /// </summary>
    /// <typeparam name="T">The module type implementing <see cref="IModule"/>.</typeparam>
    /// <param name="iconName">The name of the SVG icon file. Cannot be null or empty.</param>
    /// <returns>
    /// A <see cref="Uri"/> with <see cref="UriKind.Relative"/> kind representing the path to the module icon file.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="iconName"/> is null or empty,
    /// or if the assembly for type <typeparamref name="T"/> cannot be retrieved.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed path is not a valid relative URI.</exception>
    public static Uri GetModuleIconUrl<T>(string iconName)
        where T : IModule
    {
        ValidateFilename(iconName);
        var modulePath = GetModuleManifestName(typeof(T));
        var path = $"{ContentPrefix}/{modulePath}/svg/{iconName}";
        return new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Constructs the path prefix for asset URIs based on the relative flag.
    /// </summary>
    /// <param name="relative">If <see langword="true"/>, returns an empty string (representing absolute root <c>/</c>);
    /// otherwise returns <c>.</c> (representing the current directory).</param>
    /// <returns>The appropriate prefix for constructing relative URIs.</returns>
    private static string GetPrefix(bool relative) => relative ? "" : ".";

    /// <summary>
    /// Validates that the provided filename is not null or empty.
    /// </summary>
    /// <param name="filename">The filename to validate.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filename"/> is null or empty.</exception>
    private static void ValidateFilename(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));
    }

    /// <summary>
    /// Gets the manifest name for a module based on its type.
    /// </summary>
    /// <param name="moduleType">The module type.</param>
    /// <returns>The manifest name of the module assembly.</returns>
    /// <exception cref="ArgumentException">Thrown if the assembly for the given type cannot be retrieved.</exception>
    public static string GetModuleManifestName(Type moduleType)
    {
        var asset = Assembly.GetAssembly(moduleType);

        return asset is null
            ? throw new ArgumentException($"Failed to retrieve assembly for type {moduleType}", nameof(moduleType))
            : GetModuleManifestName(asset);
    }

    /// <summary>
    /// Gets the manifest name for a module based on its assembly.
    /// </summary>
    /// <param name="moduleAssembly">The module assembly.</param>
    /// <returns>The manifest name of the assembly, with the .dll extension removed if present.</returns>
    public static string GetModuleManifestName(Assembly moduleAssembly)
    {
        try
        {
            // Note: If assembly was loaded via zip this can fail. In such cases, we fallback to GetName().
            var calling = moduleAssembly.GetName().Name ?? moduleAssembly.ManifestModule.Name;
            if (string.IsNullOrEmpty(calling))
                throw new InvalidDataException("Calling manifest not found!");

            if (!calling.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                return calling;

            // Remove .dll extension
            return calling[..^4];
        }
        catch (InvalidDataException)
        {
            // This happens on WASM assemblies that are not forced to create a manifest.
            // We will fallback below.
        }

        var name = moduleAssembly.GetName();

        return name.Name ?? string.Empty;
    }
}
