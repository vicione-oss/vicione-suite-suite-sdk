using Sdk.Client.Models;

namespace Sdk.Client.Interfaces;

/// <summary>
/// An object that reports changes to its properties.
/// </summary>
public interface IHasChangeableProperties
{
    /// <summary>
    /// Raised when one or more properties changed.
    /// </summary>
    event Action<PropertiesChangedEventArgs>? Changed;
}
