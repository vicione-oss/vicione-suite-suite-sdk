namespace Sdk.Client.Interfaces;

/// <summary>
/// Describes an instance with an update lock.
/// </summary>
public interface IHasUpdateLock
{
    /// <summary>
    /// Gets the number of open update cycles; no event is raised while it is above 0.
    /// </summary>
    int UpdateLock { get; }

    /// <summary>
    /// Starts an update cycle that holds back change events; every call needs a matching <see cref="EndUpdate"/>.
    /// </summary>
    void BeginUpdate();

    /// <summary>
    /// Ends an update cycle; closing the last one raises the changes collected during the cycle.
    /// </summary>
    void EndUpdate();
}
