using Sdk.Client.Models;

namespace Sdk.Client.Interfaces;

/// <summary>
/// Describes an instance with changeable properties
/// </summary>
public interface IHasChangeableProperties
{
    /// <summary>
    /// Raised when one or more properties have changed
    /// </summary>
    event Action<PropertiesChangedEventArgs>? Changed;
}
