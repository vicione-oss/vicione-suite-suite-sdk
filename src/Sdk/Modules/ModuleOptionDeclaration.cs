using System.Diagnostics;

namespace Sdk.Modules;

[DebuggerDisplay("Option = {Key,nq}, Type = {OptionType,nq}, Default = {DefaultValue}")]
/// <summary>
/// Represents a declaration of a configurable option for a module,
/// including its key, current and default values, and metadata such as whether it is required or sensitive.
/// </summary>
public class ModuleOptionDeclaration
{
    /// <summary>
    /// Unique key that identifies this option
    /// </summary>
    /// <remarks>
    /// This key is required and is used to look up or store the option in configuration.
    /// </remarks>
    public required string Key { get; set; }

    /// <summary>
    /// Currently assigned value for this option, if any
    /// </summary>
    /// <remarks>
    /// If <see langword="null"/>, no explicit value has been provided and <see cref="DefaultValue"/> may be used.
    /// </remarks>
    public string? Value { get; set; }

    /// <summary>
    /// Default value for this option
    /// </summary>
    /// <remarks>
    /// The default value is used when <see cref="Value"/> is <see langword="null"/> and the option is not marked as required.
    /// </remarks>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Value indicating whether this option represents a password or other sensitive information
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/>, user interfaces may choose to mask or encrypt this value.
    /// </remarks>
    public bool IsPassword { get; set; }

    /// <summary>
    /// Value indicating whether this option must be provided by the user or system
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/>, the configuration system or validation logic should ensure that a value is present.
    /// </remarks>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Type of this option, describing how it should be interpreted or validated
    /// </summary>
    /// <remarks>
    /// The <see cref="ModuleOptionType"/> may define option categories such as text, number, boolean, etc.
    /// </remarks>
    public ModuleOptionType OptionType { get; set; }
}
