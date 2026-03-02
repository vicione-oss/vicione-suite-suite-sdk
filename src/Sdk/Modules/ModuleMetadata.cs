using System.Diagnostics;

namespace Sdk.Modules;

/// <summary>
/// Represents metadata describing a module, including its identity, versioning,
/// descriptive information, dependencies, and supported capabilities.
/// </summary>
[DebuggerDisplay("Name = {Name,nq}, Version = {Version,nq}, MinSdk = {MinSuiteSdkVersion,nq}")]
[ExcludeFromCodeCoverage]
public class ModuleMetadata
{
    /// <summary>
    /// Technical name of the module.
    /// </summary>
    /// <remarks>
    /// This name should be unique across all modules and is typically used as an identifier.
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>
    /// Optional human-friendly title for the module.
    /// </summary>
    /// <remarks>
    /// Used for UI display or documentation purposes.
    /// </remarks>
    public string? Title { get; init; }

    /// <summary>
    /// Optional brief description of what the module does or provides
    /// </summary>
    /// <remarks>
    /// Used for descriptive UI elements like summaries or tooltips.
    /// </remarks>
    public string? Description { get; init; }

    /// <summary>
    /// Version of this module.
    /// </summary>
    /// <remarks>
    /// This version should follow semantic versioning (e.g. <c>1.0.0</c>).
    /// </remarks>
    public required string Version { get; init; }

    /// <summary>
    /// Minimum suite SDK version that this module is compatible with.
    /// </summary>
    public required string MinSuiteSdkVersion { get; init; }

    /// <summary>
    /// Optional name of the company or organization that provides this module.
    /// </summary>
    public string? Company { get; init; }

    /// <summary>
    /// Optional SVG icon representing this module.
    /// </summary>
    public string? IconSvg { get; set; }

    /// <summary>
    /// Optional URL to the project website or documentation for this module.
    /// </summary>
    public Uri? ProjectUrl { get; init; }

    /// <summary>
    /// Optional URL to a README or detailed documentation for this module.
    /// </summary>
    public Uri? ReadmeUrl { get; init; }

    /// <summary>
    /// Value indicating whether this module provides backend functionality.
    /// </summary>
    public bool HasBackend { get; set; }

    /// <summary>
    /// Value indicating whether this module provides frontend functionality.
    /// </summary>
    public bool HasFrontend { get; set; }

    /// <summary>
    /// Temporary flag to indicate that this module will handle its own version updates
    /// (autonomous migration) instead of relying on the suite's migration mechanism.
    /// </summary>
    public bool AutonomousMigration { get; set; }

    /// <summary>
    /// Optional publication date of the module.
    /// </summary>
    public DateTimeOffset? Published { get; init; }

    /// <summary>
    /// Optional list of tags describing the module which can be used for searching, filtering or categorization.
    /// </summary>
    public List<string>? Tags { get; init; } = [];

    /// <summary>
    /// Optional dependency packages that this module requires.
    /// </summary>
    /// <remarks>
    /// Each entry describes another package that must be present for this module to function.
    /// </remarks>
    public List<ModuleDependencyPackage>? Dependencies { get; init; }

    /// <summary>
    /// Optional configurable options that this module exposes.
    /// </summary>
    public List<ModuleOptionDeclaration>? Options { get; init; }
}
