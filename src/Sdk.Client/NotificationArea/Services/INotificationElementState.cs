namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// State for a notification element
/// </summary>
public interface INotificationElementState
{
    /// <summary>
    /// This flag indicates the active state. For example, the element is active after it has been clicked.
    /// </summary>
    /// <remarks>
    /// If set to <see langword="true"/> then <see cref="Visible"/> is also set to <see langword="true"/>.
    ///
    /// <para>
    /// The default value is <see langword="false"/>.
    /// </para>
    /// </remarks>
    bool IsActive { get; set; }

    /// <summary>
    /// Returns true when the notification element should be visible.
    /// </summary>
    /// <remarks>
    /// If set to <see langword="false"/> then <see cref="IsActive"/> is also set to <see langword="false"/>.
    ///
    /// <para>
    /// The default value is <see langword="true"/>.
    /// </para>
    /// </remarks>
    bool Visible { get; set; }

    /// <summary>
    /// Raised when state has been changed and no <see cref="BeginUpdate">update cycle</see> is running.
    /// </summary>
    event Action<NotificationElementStateChangedEventArgs>? Changed;

    /// <summary>
    /// Call this method to begin an update cycle.
    /// </summary>
    /// <remarks>
    /// Make sure to have a corresponding call to <see cref="EndUpdate"/> to end the update cycle.
    /// </remarks>
    void BeginUpdate();

    /// <summary>
    /// Call this method to end an update cycle.
    /// </summary>
    void EndUpdate();
}
