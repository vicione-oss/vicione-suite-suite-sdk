namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// State for a control panel
/// </summary>
public interface IControlPanelState
{
    /// <summary>
    /// Holds the index of the control panel page that should be active, otherwise null when
    /// no specific control panel page should be active or no control panel pages exist.
    /// </summary>
    int? ActivePageIndex { get; set; }

    /// <summary>
    /// Returns true when the control panel is in a loading cycle started by a call to
    /// <see cref="BeginLoading"/>, otherwise returns false.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Raised when state has changed, for example the <see cref="IsLoading" /> flag.
    /// </summary>
    event Action<ControlPanelStateChangedEventArgs>? Changed;

    /// <summary>
    /// Call this method to begin a loading cycle.
    /// 
    /// This increments the internal loading counter, which initially is zero. The counter counts the number of times
    /// <see cref="BeginLoading"/> was called without a corresponding call to <see cref="EndLoading"/>.
    /// 
    /// If the internal loading counter was zero on method entry then <see cref="IsLoading" /> is set to true and
    /// the <see cref="Changed"/> event is raised.
    /// 
    /// Make sure to have a corresponding call to <see cref="EndLoading"/> to end the loading cycle.
    /// </summary>
    void BeginLoading();

    /// <summary>
    /// Call this method to end a loading cycle. This decrements the internal loading counter.
    /// 
    /// When the internal loading counter reaches zero, <see cref="IsLoading" /> is set to false and
    /// the <see cref="Changed"/> event is raised.
    /// </summary>
    void EndLoading();
}
