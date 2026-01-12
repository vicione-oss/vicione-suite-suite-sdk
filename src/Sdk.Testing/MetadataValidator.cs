using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sdk.Modules;
using Sdk.Testing.Extensions;

namespace Sdk.Testing;

/// <summary>
/// Provides static methods for validating a module's metadata file.
/// </summary>
public static class MetadataValidator
{
    private const string MetadataFileName = "module-metadata.json";

    private static JsonSerializerOptions? s_serializerOptions;

    private static JsonSerializerOptions GetSerializerOptions()
    {
        if (s_serializerOptions is null)
        {
            s_serializerOptions = new()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
            };

            s_serializerOptions.Converters.Add(new JsonStringEnumConverter());
        }
        return s_serializerOptions;
    }

    /// <summary>
    /// Validates the 'module-metadata.json' file against the SDK and specified dependency assemblies.
    /// </summary>
    public static ModuleMetadata ValidateMetadata(string? metadataFilePath = null, params Assembly[]? dependencies)
        => ValidateMetadata(new FileSystem(), metadataFilePath, dependencies);

    /// <summary>
    /// Validates the 'module-metadata.json' file against the SDK and specified dependency assemblies, using a provided file system abstraction.
    /// </summary>
    public static ModuleMetadata ValidateMetadata(IFileSystem fileSystem, string? metadataFilePath = null, params Assembly[]? dependencies)
    {
        if (string.IsNullOrWhiteSpace(metadataFilePath))
        {
            metadataFilePath = fileSystem.Path.Combine(fileSystem.GetRepositoryRootPath(), MetadataFileName);
        }

        if (!fileSystem.File.Exists(metadataFilePath))
            throw new FileNotFoundException("Metadata file could not be found.", metadataFilePath);

        // first check if serialization is working
        var metadataJson = fileSystem.File.ReadAllText(metadataFilePath);
        var metadata = JsonSerializer.Deserialize<ModuleMetadata>(metadataJson, GetSerializerOptions())
            ?? throw new InvalidOperationException("Metadata could not be deserialized.");

        // check if the referenced SDK matches the on in the metadata
        var sdkVersion = GetSdkVersion();

        if (!Version.TryParse(metadata.MinSuiteSdkVersion, out var metadataSdkVersion))
            throw new InvalidOperationException($"Metadata contains invalid version '{metadata.Version}'.");

        // compare only major, minor and build
        if (sdkVersion.Major != metadataSdkVersion.Major || sdkVersion.Minor != metadataSdkVersion.Minor)
            throw new InvalidOperationException($"Metadata version '{metadataSdkVersion}' does not match SDK version '{sdkVersion}'.");

        ValidateModuleMetadataDependencies(metadata, dependencies);

        return metadata;
    }

    private static void ValidateModuleMetadataDependencies(ModuleMetadata metadata, Assembly[]? dependencies)
    {
        if (dependencies is null)
            return;

        // todo - maybe there's a better way instead passing the referenced assembly here but we don't know
        // what to look for - crawl for the deps.json and analyze it would be cleaner
        // Maybe provide a generic call but for which module type? Backend, Client or only IModule?

        // check dependency versions
        foreach (var dependency in dependencies)
        {
            // e.g. ViciOne.Suite.ClusterManagement.Public
            var assemblyName = dependency.GetName();
            if (string.IsNullOrEmpty(assemblyName.Name) || !assemblyName.Name.EndsWith(".Public.dll", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Referenced dependency '{assemblyName.Name}' is no public module library.");

            if (metadata.Dependencies is null)
                throw new InvalidOperationException($"Referenced module assembly '{assemblyName.Name}' is missing in metadata dependencies.");

            if (assemblyName.Version is null)
                throw new InvalidOperationException($"Dependency version '{assemblyName.Name}' has no version.");

            var moduleName = assemblyName.Name.Replace(".Public.dll", "", StringComparison.Ordinal);

            // we await correct naming conventions -> referenced assembly e.g. ViciOne.Suite.ClusterManagement.Public.dll
            // so name of the module should be ViciOne.Suite.ClusterManagement
            var exists = metadata.Dependencies.FirstOrDefault(k => k.Name == moduleName)
                ?? throw new InvalidOperationException($"Referenced module '{moduleName}' is missing in metadata dependencies.");

            if (!Version.TryParse(exists.Version, out var dependencyVersion))
                throw new InvalidOperationException($"Metadata contains invalid version '{metadata.Version}'.");

            // compare only major, minor and build
            if (dependencyVersion.Major != assemblyName.Version.Major
                || dependencyVersion.Minor != assemblyName.Version.Minor
                || dependencyVersion.Build != assemblyName.Version.Build)
            {
                throw new InvalidOperationException($"Metadata dependency '{moduleName}' version '{dependencyVersion}' does not match referenced version '{assemblyName.Version}'.");
            }
        }
    }

    /// <summary>
    /// Gets the version of the currently referenced SDK assembly.
    /// </summary>
    public static Version GetSdkVersion()
    {
        var sdkAssembly = Assembly.GetAssembly(typeof(ModuleMetadata));
        var sdkName = sdkAssembly!.GetName() ?? throw new InvalidOperationException("Failed to get SDK assembly");

        if (sdkName.Version is null)
            throw new InvalidOperationException("Referenced SDK version can't be determined.");

        return sdkName.Version;
    }
}
