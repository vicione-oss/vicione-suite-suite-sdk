namespace Sdk.Client.Services;

/// <summary>
/// Registry for items of type <see cref="T"/>
/// </summary>
public interface IRegistry<T> : IEnumerable<T>
{
    /// <summary>
    /// Counts the number of times <see cref="BeginUpdate"/> was called without a corresponding call to <see cref="EndUpdate"/>.
    /// 
    /// When <see cref="UpdateLock"/> is greater than 0, the registry is not raising <see cref="Changed"/> events when methods
    /// updating the registry are called.
    /// </summary>
    int UpdateLock { get; }

    /// <summary>
    /// Raised when the registry was changed.
    /// </summary>
    event Action<RegistryChangedEventArgs<T>> Changed;

    /// <summary>
    /// Call this method to begin an update cycle.
    /// 
    /// This increments the <see cref="UpdateLock"/>.
    /// 
    /// Make sure to have a corresponding call to <see cref="EndUpdate"/> to end the update cycle.
    /// </summary>
    void BeginUpdate();

    /// <summary>
    /// Call this method to end an update cycle.
    /// 
    /// This decrements the <see cref="UpdateLock"/>.
    /// </summary>
    void EndUpdate();

    /// <summary>
    /// Removes an item from the registry.
    /// </summary>
    /// <returns>True when item was removed</returns>
    bool Remove(T item);

    /// <summary>
    /// Removes all items with the given predicate from the registry.
    /// </summary>
    /// <returns>Number of notification elements removed</returns>
    int Remove(Predicate<T> match);
}
