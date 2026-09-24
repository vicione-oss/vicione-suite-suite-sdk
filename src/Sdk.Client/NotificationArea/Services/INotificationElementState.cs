namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// The state of a notification element, kept outside the component's render cycle.
/// </summary>
public interface INotificationElementState
{
    /// <summary>
    /// Gets or sets whether the element is active, e.g. after it was clicked; setting it also makes the element
    /// <see cref="Visible"/>. Defaults to <see langword="false"/>.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether the element is shown; hiding it also deactivates it. Defaults to <see langword="true"/>.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Raised when the state changed; during an <see cref="BeginUpdate">update cycle</see> it is raised once, by the final
    /// <see cref="EndUpdate"/>.
    /// </summary>
    event Action<NotificationElementStateChangedEventArgs>? Changed;

    /// <summary>
    /// Starts an update cycle that collects changes; every call needs a matching <see cref="EndUpdate"/>.
    /// </summary>
    void BeginUpdate();

    /// <summary>
    /// Ends an update cycle; the final one raises <see cref="Changed"/> for everything changed during the cycle.
    /// </summary>
    void EndUpdate();
}
