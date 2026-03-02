namespace Sdk.Client.Services;

/// <summary>
/// Arguments for event <see cref="IRegistry{T}.Changed"/>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RegistryChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Gets the registry instance that raised the event.
    /// </summary>
    public required IRegistry<T> Sender { get; init; }

    /// <summary>
    /// Gets the collection of items that were added to the registry.
    /// </summary>
    public required IEnumerable<T> ItemsAdded { get; init; }

    /// <summary>
    /// Gets the collection of items that were removed from the registry.
    /// </summary>
    public required IEnumerable<T> ItemsRemoved { get; init; }
}
