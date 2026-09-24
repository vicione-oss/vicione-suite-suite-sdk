using Sdk.Client.Interfaces;

namespace Sdk.Client.Models;

/// <summary>
/// Arguments of <see cref="IHasChangeableProperties.Changed"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PropertiesChangedEventArgs(IHasChangeableProperties sender, IReadOnlySet<string> propertyNames) : EventArgs
{
    /// <summary>
    /// Gets the object that raised the event.
    /// </summary>
    public IHasChangeableProperties Sender => sender;

    /// <summary>
    /// Gets the names of the changed properties.
    /// </summary>
    public IReadOnlySet<string> PropertyNames => propertyNames;
}
