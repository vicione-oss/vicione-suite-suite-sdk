namespace Sdk.Modules;

/// <summary>
/// Specifies the data type of a module option.
/// </summary>
public enum ModuleOptionType
{
    /// <summary>
    /// The option is a boolean value (true/false).
    /// </summary>
    Boolean,

    /// <summary>
    /// The option is a numerical value, such as an integer or a floating-point number.
    /// </summary>
    Number,

    /// <summary>
    /// The option is a string of text.
    /// </summary>
    Text
}
