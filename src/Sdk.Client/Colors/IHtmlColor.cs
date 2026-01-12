namespace Sdk.Client.Colors;

/// <summary>
/// Defines a contract for objects that can be represented as an HTML color string.
/// </summary>
public interface IHtmlColor
{
    /// <summary>
    /// Returns the HTML-compatible color string representation of the object.
    /// </summary>
    string ToString();
}
