namespace Sdk.Client.Interfaces;

/// <summary>
/// Describes an instance with an update lock.
/// </summary>
public interface IHasUpdateLock
{
    /// <summary>
    /// Counts the number of times <see cref="BeginUpdate"/> was called without a corresponding call to <see cref="EndUpdate"/>.
    /// </summary>
    /// <remarks>
    /// The instance will not raise any event when <see cref="UpdateLock"/> is greater than 0.
    /// </remarks>
    int UpdateLock { get; }

    /// <summary>
    /// Call this method to begin an update cycle.
    /// </summary>
    /// <remarks>
    /// This increments the <see cref="UpdateLock"/>.
    ///
    /// <para>
    /// Make sure to have a corresponding call to <see cref="EndUpdate"/> to end the update cycle.
    /// </para>
    /// </remarks>
    void BeginUpdate();

    /// <summary>
    /// Call this method to end an update cycle.
    /// </summary>
    /// <remarks>
    /// This decrements the <see cref="UpdateLock"/>.
    /// </remarks>
    void EndUpdate();
}
