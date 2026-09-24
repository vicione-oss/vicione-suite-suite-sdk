namespace Sdk.Client.Services;

/// <summary>
/// Arguments of <see cref="IRegistry{T}.Changed"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RegistryChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Gets or initializes the registry that raised the event.
    /// </summary>
    public required IRegistry<T> Sender { get; init; }

    /// <summary>
    /// Gets or initializes the items added to the registry.
    /// </summary>
    public required IEnumerable<T> ItemsAdded { get; init; }

    /// <summary>
    /// Gets or initializes the items removed from the registry.
    /// </summary>
    public required IEnumerable<T> ItemsRemoved { get; init; }
}
