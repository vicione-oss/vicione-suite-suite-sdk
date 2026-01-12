using Sdk.Client.Interfaces;

namespace Sdk.Client.Services;

/// <summary>
/// Defines a generic registry for storing and managing a collection of items of type <typeparamref name="T"/>.
/// </summary>
public interface IRegistry<T> : IEnumerable<T>, IHasUpdateLock
{
    /// <summary>
    /// Raised when the registry was changed.
    /// </summary>
    event Action<RegistryChangedEventArgs<T>> Changed;

    /// <summary>
    /// Removes an item from the registry.
    /// </summary>
    /// <returns><see langword="true"/> when item was removed, otherwise <see langword="false"/>.</returns>
    bool Remove(T item);

    /// <summary>
    /// Removes all items with the given predicate from the registry.
    /// </summary>
    /// <returns>Number of notification elements removed.</returns>
    int Remove(Predicate<T> match);
}
