using System.IO.Abstractions;
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
    /// Validates <c>module-metadata.json</c> against the referenced SDK and <paramref name="dependencies"/> on the real file system.
    /// </summary>
    /// <inheritdoc cref="ValidateMetadata(IFileSystem, string?, Assembly[])"/>
    public static ModuleMetadata ValidateMetadata(string? metadataFilePath = null, params Assembly[]? dependencies)
        => ValidateMetadata(new FileSystem(), metadataFilePath, dependencies);

    /// <summary>
    /// Validates <c>module-metadata.json</c>: it must deserialize, its <c>MinSuiteSdkVersion</c> must match the referenced SDK
    /// in major and minor, and every dependency must be listed with the same major, minor and build.
    /// </summary>
    /// <param name="fileSystem">The file system to read from.</param>
    /// <param name="metadataFilePath">The metadata file; <see langword="null"/> or blank means the file in the repository root.</param>
    /// <param name="dependencies">The referenced <c>*.Public</c> module assemblies; <see langword="null"/> skips the check.</param>
    /// <returns>The deserialized metadata.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the metadata file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if deserialization or any version check fails.</exception>
    public static ModuleMetadata ValidateMetadata(IFileSystem fileSystem, string? metadataFilePath = null, params Assembly[]? dependencies)
    {
        if (string.IsNullOrWhiteSpace(metadataFilePath))
        {
            metadataFilePath = fileSystem.Path.Combine(fileSystem.GetRepositoryRootPath(), MetadataFileName);
        }

        if (!fileSystem.File.Exists(metadataFilePath))
            throw new FileNotFoundException("Metadata file could not be found.", metadataFilePath);

        var metadataJson = fileSystem.File.ReadAllText(metadataFilePath);
        var metadata = JsonSerializer.Deserialize<ModuleMetadata>(metadataJson, GetSerializerOptions())
            ?? throw new InvalidOperationException("Metadata could not be deserialized.");

        var sdkVersion = GetSdkVersion();

        if (!Version.TryParse(metadata.MinSuiteSdkVersion, out var metadataSdkVersion))
            throw new InvalidOperationException($"Metadata contains invalid version '{metadata.MinSuiteSdkVersion}'.");

        // Major and minor must match; build and revision may differ.
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

        foreach (var dependency in dependencies)
        {
            var assemblyName = dependency.GetName();
            if (string.IsNullOrEmpty(assemblyName.Name)
                || !assemblyName.Name.EndsWith(ModuleIdResolver.ModuleSuffixPublic, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Referenced dependency '{assemblyName.Name}' is no public module library.");
            }

            if (metadata.Dependencies is null)
                throw new InvalidOperationException($"Referenced module assembly '{assemblyName.Name}' is missing in metadata dependencies.");

            if (assemblyName.Version is null)
                throw new InvalidOperationException($"Dependency version '{assemblyName.Name}' has no version.");

            var moduleName = assemblyName.Name[..^ModuleIdResolver.ModuleSuffixPublic.Length];

            // Naming convention: assembly ViciOne.Suite.ClusterManagement.Public belongs to module ViciOne.Suite.ClusterManagement.
            var exists = metadata.Dependencies.FirstOrDefault(k => k.Name == moduleName)
                ?? throw new InvalidOperationException($"Referenced module '{moduleName}' is missing in metadata dependencies.");

            if (!Version.TryParse(exists.Version, out var dependencyVersion))
                throw new InvalidOperationException($"Metadata dependency '{moduleName}' has invalid version '{exists.Version}'.");

            // The revision is ignored.
            if (dependencyVersion.Major != assemblyName.Version.Major
                || dependencyVersion.Minor != assemblyName.Version.Minor
                || dependencyVersion.Build != assemblyName.Version.Build)
            {
                throw new InvalidOperationException($"Metadata dependency '{moduleName}' version '{dependencyVersion}' does not match referenced version '{assemblyName.Version}'.");
            }
        }
    }

    /// <summary>
    /// Returns the assembly version of the SDK this test project references.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the SDK assembly carries no version.</exception>
    public static Version GetSdkVersion()
    {
        var sdkAssembly = Assembly.GetAssembly(typeof(ModuleMetadata));
        var sdkName = sdkAssembly!.GetName() ?? throw new InvalidOperationException("Failed to get SDK assembly");

        if (sdkName.Version is null)
            throw new InvalidOperationException("Referenced SDK version can't be determined.");

        return sdkName.Version;
    }
}
