using System.Diagnostics;

namespace Sdk.Modules;

/// <summary>
/// Represents a declaration of a configurable option for a module,
/// including its key, current and default values, and metadata such as whether it is required or sensitive.
/// </summary>
[DebuggerDisplay("Option = {Key,nq}, Type = {OptionType,nq}, Default = {DefaultValue}")]
[ExcludeFromCodeCoverage]
public class ModuleOptionDeclaration
{
    /// <summary>
    /// Gets or sets the unique key that identifies this option.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the currently assigned value for this option, if any.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the optional default value for this option.
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets whether this option represents a password or other sensitive information.
    /// </summary>
    public bool IsPassword { get; set; }

    /// <summary>
    /// Gets or sets whether this option must be provided by the user or system.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the type of this option, describing how it should be interpreted or validated.
    /// </summary>
    public ModuleOptionType OptionType { get; set; }
}
