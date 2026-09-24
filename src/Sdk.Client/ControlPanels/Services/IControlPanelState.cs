using Sdk.Client.Interfaces;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// The state of a control panel, kept outside the component's render cycle.
/// </summary>
public interface IControlPanelState : IHasUpdateLock
{
    /// <summary>
    /// Gets or sets the index of the page to show; <see langword="null"/> when no specific page is requested or there are no pages.
    /// </summary>
    int? ActivePageIndex { get; set; }

    /// <summary>
    /// Gets whether a loading cycle started by <see cref="BeginLoading"/> is still open.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Raised when the state changed, e.g. <see cref="IsLoading"/>.
    /// </summary>
    event Action<ControlPanelStateChangedEventArgs>? Changed;

    /// <summary>
    /// Starts a loading cycle. Cycles nest: <see cref="IsLoading"/> stays <see langword="true"/> until every call has a matching
    /// <see cref="EndLoading"/>.
    /// </summary>
    /// <remarks>
    /// The first open cycle sets <see cref="IsLoading"/> and raises <see cref="Changed"/>.
    /// </remarks>
    void BeginLoading();

    /// <summary>
    /// Ends a loading cycle; closing the last one clears <see cref="IsLoading"/> and raises <see cref="Changed"/>.
    /// A call without an open cycle is ignored.
    /// </summary>
    void EndLoading();
}
