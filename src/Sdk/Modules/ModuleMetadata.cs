using System.Diagnostics;

namespace Sdk.Modules;

[DebuggerDisplay("Name = {Name,nq}, Version = {Version,nq}, MinSdk = {MinSuiteSdkVersion,nq}")]
/// <summary>
/// Represents metadata describing a module, including its identity, versioning,
/// descriptive information, dependencies, and supported capabilities.
/// </summary>
public class ModuleMetadata
{
    /// <summary>
    /// Technical name of the module
    /// </summary>
    /// <remarks>
    /// This name should be unique across all modules and is typically used as an identifier.
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>
    /// Human-friendly title for the module
    /// </summary>
    /// <remarks>
    /// Used for UI display or documentation purposes. May be <see langword="null"/> if not provided.
    /// </remarks>
    public string? Title { get; init; }

    /// <summary>
    /// Brief description of what the module does or provides
    /// </summary>
    /// <remarks>
    /// Useful for UI summaries, catalogs, or tooltips. May be <see langword="null"/>.
    /// </remarks>
    public string? Description { get; init; }

    /// <summary>
    /// Version of this module
    /// </summary>
    /// <remarks>
    /// This version should follow semantic versioning (e.g., <c>1.0.0</c>).
    /// </remarks>
    public required string Version { get; init; }

    /// <summary>
    /// Minimum suite SDK version that this module is compatible with
    /// </summary>
    public required string MinSuiteSdkVersion { get; init; }

    /// <summary>
    /// Name of the company or organization that provides this module
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not specified.
    /// </remarks>
    public string? Company { get; init; }

    /// <summary>
    /// SVG icon representing this module
    /// </summary>
    /// <remarks>
    /// Can be used in UI displays. May be <see langword="null"/> if no icon is provided.
    /// </remarks>
    public string? IconSvg { get; set; }

    /// <summary>
    /// URL to the project website or documentation for this module
    /// </summary>
    public Uri? ProjectUrl { get; init; }

    /// <summary>
    /// URL to a README or detailed documentation for this module
    /// </summary>
    public Uri? ReadmeUrl { get; init; }

    /// <summary>
    /// Value indicating whether this module provides backend functionality
    /// </summary>
    public bool HasBackend { get; set; }

    /// <summary>
    /// Value indicating whether this module provides frontend functionality
    /// </summary>
    public bool HasFrontend { get; set; }

    /// <summary>
    /// Temporary flag to indicate that this module will handle its own version updates
    /// (autonomous migration) instead of relying on the suite's migration mechanism.
    /// </summary>
    public bool AutonomousMigration { get; set; }

    /// <summary>
    /// Publication date of the module, if known
    /// </summary>
    public DateTimeOffset? Published { get; init; }

    /// <summary>
    /// List of tags describing the module which can be used for searching, filtering or categorization
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> or empty if no tags are defined.
    /// </remarks>
    public List<string>? Tags { get; init; } = [];

    /// <summary>
    /// Dependency packages that this module requires
    /// </summary>
    /// <remarks>
    /// Each entry describes another module or package that must be present for this module to function.
    /// May be <see langword="null"/> if there are no explicit dependencies.
    /// </remarks>
    public List<ModuleDependencyPackage>? Dependencies { get; init; }

    /// <summary>
    /// Configurable options that this module exposes
    /// </summary>
    /// <remarks>
    /// Each option describes a configurable setting, its default value, and constraints.
    /// May be <see langword="null"/> if the module does not expose any configurable options.
    /// </remarks>
    public List<ModuleOptionDeclaration>? Options { get; init; }
}
